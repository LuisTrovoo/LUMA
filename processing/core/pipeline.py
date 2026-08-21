"""
core/pipeline.py

Pipeline de processamento de imagens de histologia (Otsu Tradicional).
Conversão do código MATLAB pipeline_histologia.m para Python.

Esta função é totalmente independente da interface gráfica (PySide6),
o que facilita reaproveitá-la a partir de outra aplicação/linguagem
(ex.: chamando este script via subprocess/CLI a partir de uma
interface em C#).
"""

import os
import numpy as np
import cv2
from scipy import ndimage as ndi
from scipy.spatial.distance import pdist, squareform
import scipy.io as sio
from scipy.ndimage import gaussian_filter
from skimage import filters, measure, color, morphology, segmentation
from sklearn.preprocessing import StandardScaler
from sklearn.cluster import KMeans


def pipeline_core(image_path):
    """
    Conversão do código MATLAB pipeline_histologia.m para Python (Otsu Tradicional)
    """
    print("=" * 60)
    print("INICIANDO PROCESSAMENTO...")
    print(f"Imagem: {image_path}")
    print("=" * 60)

    if not os.path.isfile(image_path):
        raise FileNotFoundError(f'Caminho de imagem inválido: {image_path}')

    # ============================================================
    # 1. LEITURA DA IMAGEM
    # ============================================================
    print("1. Lendo imagem...")
    imgRGB = cv2.imread(image_path)
    if imgRGB is None:
        raise ValueError(f"Não foi possível ler a imagem: {image_path}")

    imgRGB = cv2.cvtColor(imgRGB, cv2.COLOR_BGR2RGB)
    imgRGB = imgRGB.astype(np.float64) / 255.0  # im2double

    h, w, _ = imgRGB.shape

    # ============================================================
    # 2. DECONVOLUÇÃO DE CORES
    # ============================================================
    print("2. Deconvolução de cores...")
    colorH = np.array([0.65, 0.70, 0.29])
    colorE = np.array([0.07, 0.99, 0.11])
    colorD = np.array([0.27, 0.57, 0.78])

    eps = np.finfo(float).eps
    OD_H = -np.log10(colorH + eps)
    OD_E = -np.log10(colorE + eps)
    OD_D = -np.log10(colorD + eps)

    M = np.array([OD_H, OD_E, OD_D])
    for i in range(3):
        M[i, :] = M[i, :] / np.linalg.norm(M[i, :])

    M_inv = np.linalg.inv(M.T)
    OD = -np.log10(imgRGB + eps)
    OD_reshaped = OD.reshape(-1, 3).T

    stains = M_inv @ OD_reshaped
    H = stains[0, :].reshape(h, w)

    # ============================================================
    # 3. BINARIZAÇÃO DOS NÚCLEOS (OTSU TRADICIONAL)
    # ============================================================
    print("3. Binarização dos Núcleos (Otsu Tradicional)...")

    # Normalização do Canal H (equivalente ao rescale do MATLAB)
    H_min, H_max = H.min(), H.max()
    Hn = (H - H_min) / (H_max - H_min) if H_max > H_min else np.zeros_like(H)

    # Aplicação do Otsu global (equivalente ao graythresh + imbinarize)
    try:
        T = filters.threshold_otsu(Hn)
        BW = Hn > T
    except ValueError:
        # Caso a imagem tenha variância zero (toda de uma cor)
        T = 0.0
        BW = np.zeros_like(Hn, dtype=bool)

    print(f"   Threshold Otsu: {T:.4f}")
    print(f"   Pixels brancos: {np.sum(BW)}")

    # ============================================================
    # 4. LIMPEZA MORFOLÓGICA
    # ============================================================
    print("4. Limpeza morfológica...")
    BW = morphology.remove_small_objects(BW, min_size=30, connectivity=2)
    BW = ndi.binary_fill_holes(BW)

    # ============================================================
    # 5. WATERSHED
    # ============================================================
    print("5. Watershed...")
    Ddist = ndi.distance_transform_edt(BW)

    D_s = gaussian_filter(Ddist, sigma=1)

    markers_mask = morphology.h_maxima(D_s, 1)
    markers, _ = ndi.label(markers_mask)

    L_ws = segmentation.watershed(-D_s, markers, mask=BW)

    BW_ws = BW.copy()
    BW_ws[L_ws == 0] = 0

    # ============================================================
    # 6. REMOVE CÉLULAS DAS BORDAS
    # ============================================================
    print("6. Removendo células das bordas...")
    BW_noborder = segmentation.clear_border(BW_ws)

    # ============================================================
    # 7. ROTULAÇÃO
    # ============================================================
    print("7. Rotulação...")
    L_cells, N = ndi.label(BW_noborder)

    infection_groups = []
    idx = np.array([])
    cluster_large = 0
    cluster_small = 1
    ids_large = []
    ids_small = []
    infected_counts = np.array([])
    labels_large = np.zeros_like(L_cells, dtype=bool)
    labels_small = np.zeros_like(L_cells, dtype=bool)
    infection_map = np.zeros_like(L_cells)
    centroids = []
    props = []

    if N > 0:
        # ============================================================
        # 8. ATRIBUTOS
        # ============================================================
        print("8. Extraindo atributos...")
        props = measure.regionprops(L_cells, intensity_image=H)

        diameters = np.array([2 * np.sqrt(p.area / np.pi) for p in props])
        intensityH = np.array([p.mean_intensity for p in props])

        # Corrigindo (Y, X) do Skimage para (X, Y) da interface/MATLAB
        centroids = np.array([[p.centroid[1], p.centroid[0]] for p in props])

        # ============================================================
        # 9. K-MEANS
        # ============================================================
        print("9. K-means...")
        if N >= 2:
            X = np.column_stack([diameters, intensityH])
            scaler = StandardScaler()
            Xn = scaler.fit_transform(X)

            kmeans = KMeans(n_clusters=2, n_init=5, random_state=42)
            idx = kmeans.fit_predict(Xn)

            meanDiam = [np.mean(diameters[idx == i]) for i in range(2)]
            cluster_large = np.argmax(meanDiam)
            cluster_small = 1 - cluster_large
        else:
            idx = np.zeros(N, dtype=int)
            if N == 1:
                if diameters[0] > 50:
                    cluster_large, cluster_small = 0, 1
                    idx[0] = 0
                else:
                    cluster_large, cluster_small = 1, 0
                    idx[0] = 1

        # ============================================================
        # 11. MÁSCARAS FINAIS
        # ============================================================
        print("11. Criando máscaras finais...")
        labels_large = np.zeros_like(L_cells, dtype=bool)
        labels_small = np.zeros_like(L_cells, dtype=bool)

        for i in range(1, N + 1):
            if idx[i - 1] == cluster_large:
                labels_large[L_cells == i] = True
            else:
                labels_small[L_cells == i] = True

        # ============================================================
        # 12. ASSOCIAÇÃO CENTROIDE
        # ============================================================
        print("12. Associando células...")
        ids_large = np.where(idx == cluster_large)[0]
        ids_small = np.where(idx == cluster_small)[0]

        infection_groups = [[] for _ in range(N)]
        infected_counts = np.zeros(N)
        is_assigned = np.zeros(N, dtype=bool)

        if len(centroids) > 1:
            Dcent = squareform(pdist(centroids))
        else:
            Dcent = np.array([[0]])

        for s in ids_small:
            if is_assigned[s]:
                continue

            group = [s]
            frontier = s

            while True:
                dists = Dcent[frontier, :].copy()
                dists[group] = np.inf

                if np.all(np.isinf(dists)):
                    break

                target = np.argmin(dists)

                if target in ids_large:
                    infection_groups[target].extend(group)
                    infected_counts[target] += len(group)
                    is_assigned[group] = True
                    break

                elif target in ids_small and not is_assigned[target]:
                    group = list(set(group + [target]))
                    frontier = target

                else:
                    if len(ids_large) > 0:
                        dists_to_large = Dcent[np.ix_(group, ids_large)]
                        min_per_large = np.min(dists_to_large, axis=0)
                        idx_min = np.argmin(min_per_large)
                        target_large = ids_large[idx_min]

                        infection_groups[target_large].extend(group)
                        infected_counts[target_large] += len(group)
                        is_assigned[group] = True
                    break

        # ============================================================
        # 12.5. FILTRO POR NÚMERO MÍNIMO DE PARASITAS
        # ============================================================
        print("13. Aplicando filtro mínimo...")
        MIN_PARASITES = 0

        for g in ids_large:
            if len(infection_groups[g]) < MIN_PARASITES:
                infection_groups[g] = []
                infected_counts[g] = 0

        # ============================================================
        # 13. MAPA FINAL
        # ============================================================
        print("14. Criando mapa final...")
        infection_map = np.zeros_like(L_cells)

        for g in ids_large:
            if not infection_groups[g]:
                continue
            infection_map[L_cells == (g + 1)] = (g + 1)
            for s in infection_groups[g]:
                infection_map[L_cells == (s + 1)] = (g + 1)

    # ============================================================
    # 14. SALVA RESULTADOS
    # ============================================================
    print("15. Salvando resultados...")
    folder = os.path.dirname(image_path)
    name = os.path.splitext(os.path.basename(image_path))[0]
    out_dir = os.path.join(folder, name + '_results')

    if not os.path.exists(out_dir):
        os.makedirs(out_dir)

    cv2.imwrite(os.path.join(out_dir, '01_pos_otsu.png'), (BW * 255).astype(np.uint8))
    cv2.imwrite(os.path.join(out_dir, '02_pos_imfill.png'), (ndi.binary_fill_holes(BW) * 255).astype(np.uint8))
    cv2.imwrite(os.path.join(out_dir, '03_pos_watershed.png'), (BW_ws * 255).astype(np.uint8))
    cv2.imwrite(os.path.join(out_dir, 'H_channel.png'), (Hn * 255).astype(np.uint8))
    cv2.imwrite(os.path.join(out_dir, 'mask_nuclei.png'), (BW_noborder * 255).astype(np.uint8))

    if N > 0:
        labels_color = color.label2rgb(L_cells, bg_label=0)
        cv2.imwrite(os.path.join(out_dir, 'labels.png'), (labels_color * 255).astype(np.uint8))

    stats_list = []
    if N > 0:
        for p in props:
            stats_list.append({
                'Area': p.area,
                'Centroid': p.centroid,
                'EquivDiameter': 2 * np.sqrt(p.area / np.pi),
                'Solidity': p.solidity if hasattr(p, 'solidity') else 1.0,
                'Eccentricity': p.eccentricity if hasattr(p, 'eccentricity') else 0.0,
                'MeanIntensity': p.mean_intensity
            })

    infection_groups_mat = np.empty((N, 1), dtype=object)
    for i in range(N):
        if i < len(infection_groups) and infection_groups[i]:
            infection_groups_mat[i, 0] = np.array(infection_groups[i]) + 1
        else:
            infection_groups_mat[i, 0] = np.array([])

    results_dict = {
        'stats': stats_list,
        'idx': idx + 1 if len(idx) > 0 else np.array([]),
        'cluster_large': cluster_large + 1,
        'cluster_small': cluster_small + 1,
        'infection_groups': infection_groups_mat,
        'infected_counts': infected_counts.reshape(-1, 1) if len(infected_counts) > 0 else np.array([]),
        'labels_large': labels_large,
        'labels_small': labels_small,
        'infection_map': infection_map,
        'num_large': np.sum([len(g) >= 3 for g in infection_groups]),
        'num_small': np.sum([len(g) for g in infection_groups if len(g) >= 3]),
        'MIN_PARASITES': 3
    }

    sio.savemat(os.path.join(out_dir, 'results.mat'), results_dict)

    print("=" * 60)
    print("PROCESSAMENTO CONCLUÍDO COM SUCESSO!")
    print(f"Resultados salvos em: {out_dir}")
    print("=" * 60)

    return {
        'out_dir': out_dir,
        'results_dict': results_dict,
        'L_cells': L_cells,
        'props': props,
        'idx': idx,
        'cluster_large': cluster_large,
        'cluster_small': cluster_small,
        'infection_groups': infection_groups,
        'centroids': centroids if N > 0 else [],
        'ids_large': ids_large,
        'image_path': image_path,
        'mat_file': os.path.join(out_dir, 'results.mat')
    }

import sys
import json
import numpy as np

from core.pipeline import pipeline_core
   

if __name__ == "__main__":

    if len(sys.argv) < 2:
        print("Uso: python process_image.py <caminho_da_imagem>")
        sys.exit(1)

    image_path = sys.argv[1]

    try:
        resultado = pipeline_core(image_path)

        results_dict = resultado["results_dict"]

        idx = resultado["idx"]

        numero_celulas = len(idx)

        numero_celulas_grandes = int(
            np.sum(idx == resultado["cluster_large"])
        )

        numero_celulas_pequenas = int(
            np.sum(idx == resultado["cluster_small"])
        )

        numero_infectadas = int(
            sum(
                1
                for grupo in resultado["infection_groups"]
                if len(grupo) >= 3
            )
        )

        # ============================================================
        # CENTROIDES DAS CÉLULAS
        # ============================================================

        centroides = resultado.get("centroids", [])

        centroides_json = []

        for ponto in centroides:
            centroides_json.append({
                "x": float(ponto[0]),
                "y": float(ponto[1])
            })

        # ============================================================
        # CÉLULAS GRANDES
        # ============================================================

        celulas_grandes = []

        for i, classe in enumerate(idx):
            if classe == resultado["cluster_large"]:
                celulas_grandes.append(i)

        # ============================================================
        # RESULTADO
        # ============================================================

        dados = {
            "numero_celulas": numero_celulas,

            "numero_celulas_grandes":
                numero_celulas_grandes,

            "numero_celulas_pequenas":
                numero_celulas_pequenas,

            "numero_infectadas":
                numero_infectadas,

            "centroides":
                centroides_json,

            "celulas_grandes":
                celulas_grandes,

            "pasta_resultados":
                resultado["out_dir"],

            "arquivo_mat":
                resultado["mat_file"]
        }

        arquivo_json = resultado["out_dir"] + "/result.json"

        with open(
            arquivo_json,
            "w",
            encoding="utf-8"
        ) as arquivo:

            json.dump(
                dados,
                arquivo,
                indent=4,
                ensure_ascii=False
            )

        print()
        print("=" * 60)
        print("RESULTADO JSON GERADO")
        print("=" * 60)
        print(arquivo_json)
        print("=" * 60)

    except Exception as e:

        print(f"ERRO: {e}")

        sys.exit(1)
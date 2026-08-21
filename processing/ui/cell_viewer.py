"""
ui/cell_viewer.py

Widget de visualização/edição interativa das células detectadas
(grandes e pequenas) sobre a imagem original.
"""

import numpy as np

from PySide6.QtWidgets import (
    QGraphicsView, QGraphicsScene, QGraphicsTextItem, QGraphicsEllipseItem,
    QRubberBand, QMessageBox
)
from PySide6.QtGui import QPixmap, QPainter, QColor, QPen, QFont
from PySide6.QtCore import Qt, QPoint, QRect, QSize


class CellViewer(QGraphicsView):
    def __init__(self, gabarito_widget):
        super().__init__()

        self.scene = QGraphicsScene(self)
        self.setScene(self.scene)

        self.setTransformationAnchor(QGraphicsView.NoAnchor)
        self.setResizeAnchor(QGraphicsView.NoAnchor)
        self.setHorizontalScrollBarPolicy(Qt.ScrollBarAlwaysOff)
        self.setVerticalScrollBarPolicy(Qt.ScrollBarAlwaysOff)
        self.setDragMode(QGraphicsView.NoDrag)
        self.setRenderHint(QPainter.Antialiasing)

        self.base_pixmap = None
        self.base_item = None

        self.centroids = []
        self.ids_large = []
        self.visual_groups = {}
        self.removed_cells = []
        self.selectedSmalls = set()
        self.selectedLarge = None

        self.colors = []
        self.gabarito_widget = gabarito_widget

        self.history = []

        self.rubberBand = QRubberBand(QRubberBand.Rectangle, self)
        self.origin = QPoint()
        self.zooming = False

    def mousePressEvent(self, event):
        if event.button() == Qt.LeftButton:
            self.origin = event.pos()
            self.rubberBand.setGeometry(QRect(self.origin, QSize()))
            self.rubberBand.show()
            self.zooming = True
        super().mousePressEvent(event)

    def mouseMoveEvent(self, event):
        if self.zooming:
            rect = QRect(self.origin, event.pos()).normalized()
            self.rubberBand.setGeometry(rect)
        super().mouseMoveEvent(event)

    def mouseReleaseEvent(self, event):
        if self.zooming:
            self.rubberBand.hide()
            rect = QRect(self.origin, event.pos()).normalized()

            if rect.width() > 10 and rect.height() > 10:
                scene_rect = self.mapToScene(rect).boundingRect()
                self.fitInView(scene_rect, Qt.KeepAspectRatio)

            self.zooming = False
        super().mouseReleaseEvent(event)

    def reset_view(self):
        if self.base_item:
            self.fitInView(self.base_item.boundingRect(), Qt.KeepAspectRatio)

    def load_results(self, results_data):
        print("\n" + "=" * 60)
        print("VIEWER: Carregando resultados...")
        print("=" * 60)

        self.scene.clear()

        self.centroids = results_data['centroids']
        self.ids_large = results_data['ids_large']
        infection_groups = results_data['infection_groups']

        print(f"   Centróides: {len(self.centroids)}")
        print(f"   IDs grandes: {len(self.ids_large)}")

        if len(self.centroids) == 0:
            print("   AVISO: Nenhuma célula encontrada!")
            QMessageBox.warning(None, "Aviso",
                              "Nenhuma célula foi detectada na imagem.\n"
                              "Isso pode ocorrer se a imagem não tiver núcleos visíveis.")

        self.visual_groups = {g: [] for g in self.ids_large}
        for g in self.ids_large:
            if g < len(infection_groups):
                group = infection_groups[g]
                if isinstance(group, (list, np.ndarray)):
                    for s in np.array(group).flatten():
                        if isinstance(s, (int, np.integer)):
                            self.visual_groups[g].append(int(s))

        self.removed_cells = []
        self.selectedSmalls = set()
        self.selectedLarge = None
        self.history = []
        self._save_state()

        self.colors = self.generate_distinct_colors(len(self.ids_large))

        self.base_pixmap = QPixmap(results_data['image_path'])
        if self.base_pixmap.isNull():
            print(f"   ERRO: Não foi possível carregar a imagem: {results_data['image_path']}")
        else:
            print(f"   Imagem carregada: {results_data['image_path']}")

        self.base_item = self.scene.addPixmap(self.base_pixmap)
        self.scene.setSceneRect(self.base_item.boundingRect())

        self.reset_view()
        self.draw()

        print("VIEWER: Carregamento concluído!")
        print("=" * 60)

    def generate_distinct_colors(self, n):
        palette = [
            QColor(255, 0, 0), QColor(0, 0, 255), QColor(0, 255, 0),
            QColor(255, 255, 0), QColor(255, 0, 255), QColor(0, 255, 255),
            QColor(255, 128, 0), QColor(128, 0, 255), QColor(255, 0, 128),
            QColor(0, 255, 128), QColor(128, 255, 0), QColor(0, 128, 255),
            QColor(255, 128, 128), QColor(128, 255, 128), QColor(128, 128, 255),
            QColor(255, 255, 128), QColor(255, 128, 255), QColor(128, 255, 255),
            QColor(192, 0, 0), QColor(0, 192, 0), QColor(0, 0, 192),
            QColor(192, 192, 0), QColor(192, 0, 192), QColor(0, 192, 192),
        ]

        return palette[:n] if n <= len(palette) else palette + [
            QColor.fromHsv((137 * i) % 360, 200, 220)
            for i in range(n - len(palette))
        ]

    def _save_state(self):
        state = {
            'visual_groups': {k: v.copy() for k, v in self.visual_groups.items()},
            'removed_cells': self.removed_cells.copy(),
            'selectedSmalls': self.selectedSmalls.copy(),
            'selectedLarge': self.selectedLarge
        }
        self.history.append(state)
        if len(self.history) > 20:
            self.history.pop(0)

    def undo(self):
        if len(self.history) > 1:
            self.history.pop()
            prev_state = self.history[-1]

            self.visual_groups = {k: v.copy() for k, v in prev_state['visual_groups'].items()}
            self.removed_cells = prev_state['removed_cells'].copy()
            self.selectedSmalls = prev_state['selectedSmalls'].copy()
            self.selectedLarge = prev_state['selectedLarge']

            self.draw()
            return True
        return False

    def toggle_select_small(self, sid):
        if sid in self.removed_cells:
            return

        if sid in self.selectedSmalls:
            self.selectedSmalls.remove(sid)
        else:
            self.selectedSmalls.add(sid)

        self.selectedLarge = None
        self.draw()

    def select_large_cell(self, gid):
        if gid in self.removed_cells:
            return

        if self.selectedLarge == gid:
            self.selectedLarge = None
        else:
            self.selectedLarge = gid
            self.selectedSmalls.clear()

        self.draw()

    def select_all_smalls_in_group(self, gid):
        self.selectedSmalls.clear()
        for sid in self.visual_groups[gid]:
            if sid not in self.removed_cells:
                self.selectedSmalls.add(sid)

        self.selectedLarge = None
        self.draw()

    def clear_selection(self):
        self.selectedSmalls.clear()
        self.selectedLarge = None
        self.draw()

    def associate_selected_to_large(self, large_id):
        if not self.selectedSmalls:
            return

        self._save_state()

        for sid in list(self.selectedSmalls):
            for g in self.visual_groups:
                if sid in self.visual_groups[g]:
                    self.visual_groups[g].remove(sid)

            self.visual_groups[large_id].append(sid)

        self.selectedSmalls.clear()
        self.draw()

    def delete_selected_cells(self):
        cells_to_remove = []

        if self.selectedLarge is not None:
            cells_to_remove.append(self.selectedLarge)
            for sid in self.visual_groups[self.selectedLarge]:
                if sid not in self.removed_cells:
                    cells_to_remove.append(sid)

        for sid in self.selectedSmalls:
            if sid not in self.removed_cells:
                cells_to_remove.append(sid)

        if not cells_to_remove:
            return

        self._save_state()

        for cell_id in cells_to_remove:
            if cell_id not in self.removed_cells:
                self.removed_cells.append(cell_id)

                if cell_id not in self.ids_large:
                    for g in self.visual_groups:
                        if cell_id in self.visual_groups[g]:
                            self.visual_groups[g].remove(cell_id)

        self.selectedSmalls.clear()
        self.selectedLarge = None
        self.draw()

    def draw(self):
        for item in self.scene.items():
            if item is not self.base_item:
                self.scene.removeItem(item)

        label_counter = 1
        gabarito = []

        for i, g in enumerate(self.ids_large):
            if g in self.removed_cells:
                continue

            smalls = [s for s in self.visual_groups[g] if s not in self.removed_cells]
            if len(smalls) < 2:
                continue

            color = self.colors[i % len(self.colors)]
            label = f"A{label_counter}"

            xg, yg = self.centroids[g]
            txt = QGraphicsTextItem(label)
            txt.setDefaultTextColor(color)

            f = QFont()
            f.setBold(True)
            f.setPointSize(14)
            txt.setFont(f)

            txt.setPos(xg - 16, yg - 16)
            txt.setZValue(10)

            def make_large_handler(gid):
                def handler(event):
                    if event.modifiers() == Qt.ShiftModifier:
                        self.associate_selected_to_large(gid)
                    elif event.modifiers() == Qt.ControlModifier:
                        self.select_all_smalls_in_group(gid)
                    else:
                        self.select_large_cell(gid)
                return handler

            txt.mousePressEvent = make_large_handler(g)

            if self.selectedLarge == g:
                highlight = QGraphicsEllipseItem(xg - 20, yg - 20, 40, 40)
                highlight.setPen(QPen(QColor("red"), 3))
                highlight.setBrush(Qt.transparent)
                highlight.setZValue(8)
                self.scene.addItem(highlight)

            self.scene.addItem(txt)

            for s in smalls:
                if s in self.removed_cells or s >= len(self.centroids):
                    continue

                xs, ys = self.centroids[s]
                ell = QGraphicsEllipseItem(xs - 5, ys - 5, 10, 10)
                ell.setPen(QPen(color, 2))
                ell.setBrush(Qt.transparent)
                ell.setZValue(5)

                def make_small_handler(sid):
                    def handler(event):
                        self.toggle_select_small(sid)
                    return handler

                ell.mousePressEvent = make_small_handler(s)
                self.scene.addItem(ell)

            gabarito.append((label, len(smalls), color))
            label_counter += 1

        for sid in self.selectedSmalls:
            if sid in self.removed_cells or sid >= len(self.centroids):
                continue

            xs, ys = self.centroids[sid]
            sel = QGraphicsEllipseItem(xs - 10, ys - 10, 20, 20)
            sel.setPen(QPen(QColor("yellow"), 3))
            sel.setBrush(Qt.transparent)
            sel.setZValue(20)
            self.scene.addItem(sel)

            if len(self.selectedSmalls) > 1:
                num = QGraphicsTextItem(str(len(self.selectedSmalls)))
                num.setDefaultTextColor(QColor("yellow"))
                num.setFont(QFont("Arial", 10, QFont.Bold))
                num.setPos(xs + 12, ys - 15)
                num.setZValue(25)
                self.scene.addItem(num)

        self.update_gabarito(gabarito)

    def update_gabarito(self, gabarito):
        self.gabarito_widget.clear()

        if not gabarito:
            self.gabarito_widget.append("Nenhuma célula com ≥ 2 infectoras")
        else:
            for label, n, color in gabarito:
                self.gabarito_widget.setTextColor(color)
                self.gabarito_widget.append(f"{label} – {n} células")

        self.gabarito_widget.setTextColor(QColor("black"))

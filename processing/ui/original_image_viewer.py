"""
ui/original_image_viewer.py

Visualizador simples com zoom (rubber band) para a imagem original,
usado na janela "Visualizar Imagem Original".
"""

from PySide6.QtWidgets import QGraphicsView, QGraphicsScene, QRubberBand
from PySide6.QtGui import QPixmap, QPainter
from PySide6.QtCore import Qt, QPoint, QRect, QSize


class OriginalImageViewer(QGraphicsView):
    def __init__(self):
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

        self.rubberBand = QRubberBand(QRubberBand.Rectangle, self)
        self.origin = QPoint()
        self.zooming = False

    def load_image(self, image_path):
        self.scene.clear()

        self.base_pixmap = QPixmap(image_path)
        if self.base_pixmap.isNull():
            return False

        self.base_item = self.scene.addPixmap(self.base_pixmap)
        self.scene.setSceneRect(self.base_item.boundingRect())

        self.reset_view()
        return True

    def reset_view(self):
        if self.base_item:
            self.fitInView(self.base_item.boundingRect(), Qt.KeepAspectRatio)

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

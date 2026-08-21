"""
ui/main_window.py

Janela principal da aplicação: monta o layout, conecta os botões/atalhos
e orquestra o worker de processamento e os viewers.
"""

import os

from PySide6.QtWidgets import (
    QMainWindow, QWidget, QLabel, QTextEdit, QPushButton, QVBoxLayout,
    QHBoxLayout, QFileDialog, QMessageBox, QDialog
)
from PySide6.QtGui import QFont, QKeySequence, QShortcut
from PySide6.QtCore import Qt

from workers.processing_worker import ProcessingWorker
from ui.cell_viewer import CellViewer
from ui.original_image_viewer import OriginalImageViewer


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Protótipo Aplicativo - Análise de Imagens de Histologia")
        self.resize(1500, 900)

        self.gabarito = QTextEdit()
        self.gabarito.setReadOnly(True)

        self.viewer = CellViewer(self.gabarito)
        self.current_image_path = None

        self.faculdade_label = QLabel("Grupo de Imagens Médicas\nFaculdade De Engenharia Elétrica\nUniversidade Federal de Uberlândia")
        self.faculdade_label.setAlignment(Qt.AlignCenter)

        font = QFont()
        font.setPointSize(8)
        font.setBold(True)
        self.faculdade_label.setFont(font)
        self.faculdade_label.setStyleSheet("color: white;")
        self.faculdade_label.setWordWrap(True)
        self.faculdade_label.setContentsMargins(5, 10, 5, 10)

        self.info_label = QLabel("\nINFORMAÇÕES")
        self.info_label.setAlignment(Qt.AlignCenter)
        info_font = QFont()
        info_font.setBold(True)
        info_font.setPointSize(12)
        self.info_label.setFont(info_font)
        self.info_label.setStyleSheet("color: white;")

        self.info_text = QTextEdit()
        self.info_text.setReadOnly(True)
        self.info_text.setMaximumHeight(120)
        self.info_text.setStyleSheet("""
            QTextEdit {
                background-color: transparent;
                border: none;
                color: white;
                font-size: 10pt;
            }
        """)

        self.info_text.append("• Clique em uma célula para selecioná-la")
        self.info_text.append("• Shift + clique em uma célula grande: associa a ela as outras células selecionadas")
        self.info_text.append("• Ctrl + clique em uma célula grande: seleciona todo o grupo pertencente a ela")

        self.btn_img = QPushButton("📂 Selecionar Imagem e Executar")
        self.btn_reset = QPushButton("↩ Reset View")
        self.btn_undo = QPushButton("⎌ Desfazer (Ctrl+Z)")
        self.btn_delete = QPushButton("🗑️ Excluir Célula")
        self.btn_view_original = QPushButton("Visualizar Imagem Original")

        self.btn_img.clicked.connect(self.select_image_and_run)
        self.btn_reset.clicked.connect(self.viewer.reset_view)
        self.btn_undo.clicked.connect(self.undo_action)
        self.btn_delete.clicked.connect(self.delete_cells)
        self.btn_view_original.clicked.connect(self.view_original_image)

        QShortcut(QKeySequence("Ctrl+Z"), self).activated.connect(self.undo_action)
        QShortcut(QKeySequence("Delete"), self).activated.connect(self.delete_cells)
        QShortcut(QKeySequence("Backspace"), self).activated.connect(self.delete_cells)

        left = QVBoxLayout()

        lbl = QLabel("GABARITO")
        lbl.setAlignment(Qt.AlignCenter)
        f = lbl.font()
        f.setBold(True)
        f.setPointSize(13)
        lbl.setFont(f)
        lbl.setStyleSheet("color: white;")

        left.addWidget(lbl)
        left.addWidget(self.gabarito)
        left.addWidget(self.btn_img)
        left.addWidget(self.btn_reset)
        left.addWidget(self.btn_undo)
        left.addWidget(self.btn_delete)
        left.addWidget(self.btn_view_original)

        left.addSpacing(10)
        left.addWidget(self.info_label)
        left.addWidget(self.info_text)
        left.addStretch()
        left.addWidget(self.faculdade_label)
        left.addSpacing(10)

        container = QWidget()
        layout = QHBoxLayout(container)
        layout.addLayout(left, 1)
        layout.addWidget(self.viewer, 4)

        self.setCentralWidget(container)

        self.processing_worker = None

    def undo_action(self):
        self.viewer.undo()

    def delete_cells(self):
        self.viewer.delete_selected_cells()

    def view_original_image(self):
        if not self.current_image_path:
            QMessageBox.information(self, "\nInformação",
                                  "Nenhuma imagem carregada ainda.\n"
                                  "Selecione uma imagem primeiro.")
            return

        image_window = QDialog(self)
        image_window.setWindowTitle(f"Imagem Original: {os.path.basename(self.current_image_path)}")
        image_window.resize(1000, 700)

        layout = QVBoxLayout(image_window)

        original_viewer = OriginalImageViewer()

        if not original_viewer.load_image(self.current_image_path):
            QMessageBox.critical(self, "Erro", "Não foi possível carregar a imagem original.")
            return

        button_container = QWidget()
        button_layout = QHBoxLayout(button_container)
        button_layout.setContentsMargins(0, 0, 0, 0)

        btn_reset_zoom = QPushButton("↩ Resetar Zoom")
        btn_reset_zoom.clicked.connect(original_viewer.reset_view)

        btn_close = QPushButton("Fechar Janela")
        btn_close.clicked.connect(image_window.close)

        button_layout.addWidget(btn_reset_zoom)
        button_layout.addStretch()
        button_layout.addWidget(btn_close)

        layout.addWidget(original_viewer)
        layout.addWidget(button_container)

        image_window.exec()

    def select_image_and_run(self):
        file, _ = QFileDialog.getOpenFileName(
            self, "Imagem", "", "Imagens (*.png *.jpg *.jpeg *.tif *.tiff)"
        )
        if not file:
            return

        self.image_path = file
        self.current_image_path = file

        self.gabarito.clear()
        self.gabarito.append("Processando imagem...")

        self.processing_worker = ProcessingWorker(file)
        self.processing_worker.finished.connect(self.processing_finished)
        self.processing_worker.error.connect(self.processing_error)
        self.processing_worker.start()

    def processing_finished(self, results_data):
        self.gabarito.clear()
        self.viewer.load_results(results_data)

    def processing_error(self, error_msg):
        self.gabarito.clear()
        self.gabarito.append("ERRO NO PROCESSAMENTO:")
        self.gabarito.append(error_msg[:500])
        QMessageBox.critical(self, "Erro no Processamento",
                           f"Erro ao processar a imagem.\n\nDetalhes:\n{error_msg[:200]}")

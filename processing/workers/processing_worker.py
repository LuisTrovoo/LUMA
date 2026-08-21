"""
workers/processing_worker.py

Worker (QThread) responsável por rodar o pipeline de processamento
em segundo plano, sem travar a interface gráfica.
"""

import traceback

from PySide6.QtCore import QThread, Signal

from core.pipeline import pipeline_core


class ProcessingWorker(QThread):
    finished = Signal(object)
    error = Signal(str)

    def __init__(self, image_path):
        super().__init__()
        self.image_path = image_path

    def run(self):
        try:
            print("\n" + "=" * 60)
            print("WORKER: Iniciando processamento...")
            print("=" * 60)

            results = pipeline_core(self.image_path)

            print("\n" + "=" * 60)
            print("WORKER: Processamento finalizado, emitindo resultados...")
            print("=" * 60)

            self.finished.emit(results)

        except Exception as e:
            error_details = traceback.format_exc()
            print(f"\nERRO NO WORKER:\n{error_details}")
            self.error.emit(f"{str(e)}\n\n{error_details}")

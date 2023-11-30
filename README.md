# InteractiveBook

This is an interactive book that reads pages via a web camera and displays the image on the screen.

The project was developed for a stand with a projector that displays the image on the screen.

Interactive book
The project has two modes:

1) Page Reading Mode via Tesseract (does not work stably), but if you play with calibration and clearing the image of noise (this setting is present), then stable operation can be achieved.

2) Mode for reading pages via KR code. It works stably, there is also calibration.

The project has a library that has a simple admin mode and a reader mode

Reader Mode: You can browse books in your library.

Administrator mode: You can manage the library (delete and add new books); books are added in PDF format. Add books to favorites.


For the project to work correctly, the following assets are required:

1) OpenCV+ Unity
2) Animated Loading Icons
3) Paroxe PDF

# B2VideoUploader

Tool to upload videos to BlackBlaze B2 for consumption by Cytube after converting them to a web compatiable video format

### To run

1. install git submodules
2. copy settings.sample.ini -> settings.ini
3. fill out BlackBlaze B2 credentials in settings.ini

### Features

- English subtitle extraction and upload
- Automatically creates and uploads cytube json metadata file

### TODO

- Fix Retry Mechanism
- Refactor video processing logic into VideoProcessorService
- Use events or mediators instead of passing callbacks
- Clean up code
- clean up half uploads
- Write tests

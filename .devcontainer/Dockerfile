FROM mcr.microsoft.com/devcontainers/python:1-3.11-bullseye
WORKDIR /temp
RUN apt-get update && pip install --upgrade pip && pip install pyPEG2 Arpeggio pyyaml
RUN git clone https://gitea.pep.foundation/fdik/yml2.git && cd yml2 && python3 setup.py install && python3 setup.py build
ENV PATH="${PATH}:/temp/yml2/build/scripts-3.11/"
WORKDIR /workspaces

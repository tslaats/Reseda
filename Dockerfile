#FROM fsharp
FROM frolvlad/alpine-mono
WORKDIR /reseda
COPY Reseda.REST/bin/Release .
COPY frontend/build ./build
EXPOSE 8080
CMD ["mono", "Reseda.REST.exe"]

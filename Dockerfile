#FROM microsoft/dotnet:runtime
FROM fsharp
WORKDIR /reseda
COPY Reseda.REST/bin/Release .
COPY frontend/build ./build
EXPOSE 80
CMD ["mono", "Reseda.REST.exe"]

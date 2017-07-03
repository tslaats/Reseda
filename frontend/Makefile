HOST=dcr.itu.dk

BIN=../Reseda.REST/bin/Release

deploy-dcr: build
	cd $(BIN) ; rsync -r --progress ./ $(HOST):/var/aspnet/reseda
	rsync -r --progress build/ $(HOST):/var/www/html/reseda

build-docker: 
	cp -r build $(BIN)
	cp Dockerfile $(BIN)
	cd $(BIN) ; docker build -t reseda

test-docker: build-docker
	docker run --name re reseda -p 8080:8080

docker.zip: build build-docker
	cp Dockerfile $(BIN)
	zip -j $(BIN)/*

build: 
	cd .. ; msbuild /p:Configuration=Release
	yarn build


.PHONY: deploy build


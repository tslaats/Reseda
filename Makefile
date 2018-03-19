IMAGE=reseda
USER=debois
HOST=dcr.itu.dk
REMOTE=$(USER)@$(HOST)

docker: build
	docker build -t $(IMAGE) .

# Try restore if build fails after checkout
restore:
	nuget restore
	cd frontend && yarn install

build: 
	msbuild /p:Configuration=Release
	cd frontend/src && ./generate-examples.sh
	cd frontend && yarn build

docker-run:
	sudo docker run -p 8080:8080 $(IMAGE)

docker-stop:
	sudo docker kill $(IMAGE)
	sudo docker rm $(IMAGE)

upload: docker
	docker save $(IMAGE) | bzip2 --best | pv | ssh debois@dcr.itu.dk 'bunzip2 > $(IMAGE).docker'

deploy: 
	ssh $(REMOTE) "sudo docker kill reseda"
	ssh $(REMOTE) "sudo docker rm reseda"
	ssh $(REMOTE) "sudo docker load -i $(IMAGE).docker"
	ssh $(REMOTE) "sudo docker run --name reseda -p 8025:8080 --restart always --detach $(IMAGE)"

publish: upload deploy

.phony : build restore docker publish



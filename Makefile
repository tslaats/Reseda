IMAGE=reseda

docker: build
	docker build -t $(IMAGE) .

# Try restore if build fails after checkout
restore:
	nuget restore
	cd frontend && yarn install

build: 
	msbuild /p:Configuration=Release
	cd frontend && yarn build

docker-run:
	sudo docker run -p 8080:8080 $(IMAGE)

docker-stop:
	sudo docker kill $(IMAGE)
	sudo docker rm $(IMAGE)

publish: docker
	docker save $(IMAGE) | bzip2 --best | pv | ssh debois@dcr.itu.dk 'bunzip2 > $(IMAGE).docker'
	@echo "** Recommended on remote host:"
	@echo "sudo docker kill reseda"
	@echo "sudo docker rm reseda"
	@echo "sudo docker load -i $(IMAGE).docker"
	@echo "sudo docker run --name reseda -p 8025:8080 --restart always --detach $(IMAGE)"

.phony : build restore docker



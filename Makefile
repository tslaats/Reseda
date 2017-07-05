docker: 
	msbuild /p:Configuration=Release
	cd frontend && yarn build
	#zip docker.zip Makefile Dockerfile frontend/build Reseda.REST/bin/Release 
	docker build -t debois/reseda .

docker-run:
	docker run -p 80:8080 


# Digitalocean hosting
DROPLET=reseda-droplet
CONTAINER=reseda-container

droplet-create:
	@if [ -z "$$TOKEN" ]; then  \
		echo "Set TOKEN to Digitalocean API access token\n(See ~/.docker/machine/machines/reseda-droplet/config.json)" ; \
		exit 255 ; \
	fi
	docker-machine create \
	  --driver digitalocean \
	  --digitalocean-access-token $(TOKEN) \
	  --digitalocean-image "ubuntu-14-04-x64" \
	  --digitalocean-region "ams3" \
	  $(DROPLET)
	
# Region selects "Amsterdam 3"

droplet-ip:
	docker-machine ip $(DROPLET)

droplet-kill: 
	yes | docker-machine stop -f $(DROPLET) 
	yes | docker-machine rm $(DROPLET)

# Run for initial run
container-run:
	eval $$(docker-machine env $(DROPLET)) && \
		docker run -d -p 80:8080 --name $(CONTAINER) debois/reseda

# Start for subsequent runs
container-start:
	eval $$(docker-machine env $(DROPLET)) && \
		docker start $(CONTAINER) 

container-stop:
	eval $$(docker-machine env $(DROPLET)) && \
		docker stop -t0 $(CONTAINER)

container-reload: droplet-start droplet-stop


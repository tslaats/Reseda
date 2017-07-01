HOST=dcr.itu.dk

deploy-rest: 
	cd .. ; msbuild /p:Configuration=Release
	cd ../Reseda.REST/bin/Release ; rsync -r --progress ./ $(HOST):/var/aspnet/reseda
	yarn build
	rsync -r --progress build/ $(HOST):/var/www/html/reseda


.PHONY: deploy 


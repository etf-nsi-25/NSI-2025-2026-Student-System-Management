### Local Dockerization
You will notice we are **_not_** starting the app itself as a docker
container when running locally. This is due to added complexity of setting up development tools (IDEs etc.) to work with app within containers. <br />
Docker compose here is meant primarily for _tools_. Those tools now 
include only **Postgres** but space remains for there to be future ones.

### Deployment Dockerization
At deployment time we will build a different set of Dockerfiles and files necessary for 
wherever the app will run (ECS? Kuberenets? Remains to be chosen). **This part should then be updated.**

docker run -it -p 127.0.0.1:8082:8080 -v "$PWD:/home/coder/project" -u "$(id -u):$(id -g)" codercom/code-server:latest

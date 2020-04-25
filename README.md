![Image of BetaFast](https://github.com/NetSPI/BetaFastExamples/blob/master/GUI/BetaFast/BetaFast/Images/logo_name.png)
# BetaFast
## The Company
BetaFast is the provider of a premier Betamax rental kiosk. Browse the wide selection of movies and begin renting today!

## Releases
The following versions of BetaFast have been released in conjunction with our blog series Introduction to Hacking Thick Clients:
* GUI - [Blog URL]

## The Client
To use the client, open the BetaFast solution in visual studio and compile the source code.

## The Server
Ensure that Docker is installed and that there are no conflicts with Hyper-V. Docker files can be edited to configure the database credentials, database server address and port, and the web server address and port. **Do not modify db-init.sql table formats unless you're prepared to modify how the API works.**

Docker should be configured to have a good amount of RAM and other settings. If it and the host machine lack the resources to serve data quickly, there will be weird timeouts in the client. A lot of large images are retrieved on initial login. If there are issues creating the container and connecting with sa, increase the sleep command time in db-init.sh.

To launch the servers, use the following commands in the same directory as docker-compose.yml:

```docker-compose build```

```docker-compose up```

When testing is completed, stop the containers using Ctrl - C and then type `docker-compose down`.

Note - by default, the web server is available on 127.0.0.1:8080. Therefore, if testing with docker on the same machine as the client, do not run a system proxy on 127.0.0.1:8080. Also, I like to modify the hosts file to have www.betafast.com resolve to 127.0.0.1. I then change the BetaFast client to point to http://www.betafast.com:8080 in the configuration file.

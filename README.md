![Image of BetaFast](https://github.com/NetSPI/BetaFast/blob/master/docs/images/betafast-logo.png)
# BetaFast
## The Company
BetaFast is the provider of a premier Betamax rental kiosk. Browse the wide selection of movies and begin renting today!

## Releases
Two versions of BetaFast have been released. One is written with three-tier architecture, the other two-tier architecture. Both releases contains but is not limited to the following vulnerabilities:
* Hardcoded Encryption Data
* Hardcoded Encrypted Password
* SQL Injection
* Authorization Bypass
* Missing server-side input validation
* Cleartext Password Stored - Registry
* Cleartext Sensitive Data Stored - Files
* Weak File Upload Controls
* Weak Input Validation
* No Code Obfuscation

The two-tier release contains but is not limited to the following additional vulnerabilities:
* Unencrypted Database Connection
* Hardcoded Connection String

BetaFast was developed in conjunction with our blog series Introduction to Hacking Thick Clients. An overview and further instructions can be found at https://blog.netspi.com/introducing-betafast/.

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

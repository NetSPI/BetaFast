FROM mcr.microsoft.com/mssql/server:2017-CU13
COPY . /

RUN chmod +x /db-init.sh
CMD /bin/bash ./entrypoint.sh
#wait for the SQL Server to come up
chmod +x /wait-for-it.sh
/bin/bash ./wait-for-it.sh 127.0.0.1:1433 -t 60

mkdir /var/opt/mssql/ReplData/
chown mssql /var/opt/mssql/ReplData/
chgrp mssql /var/opt/mssql/ReplData/

echo "running set up script"
sleep 10s
#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S 127.0.0.1 -U sa -P Password123 -d master -i db-init.sql
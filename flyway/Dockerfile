FROM flyway/flyway

COPY ./migrate.sh /flyway/migrate.sh

COPY ./conf /flyway/conf

COPY ./sql /flyway/sql

ENTRYPOINT [ "sh", "/flyway/migrate.sh" ]
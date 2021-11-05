FROM postgres:14.0

WORKDIR /schema

COPY ./scripts/*.sql schema/

CMD [ "psql", "-h localhost -d PickUpGames -U admin -p 5432 -a -q -f /schema/postgresql.sql"]
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY,
    name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

INSERT INTO users (id, name, email, password, created_at, updated_at)
VALUES ('52f41ff1-39bf-433b-9c07-23e1f080c802', 'John Doe', 'john.doe@email.com', '0y5XfGI92Iiy5coW+pRufQ==#PF5w0nYJLQs9KDB+epzqLJzkbP5vqoSnEYK5IGA1cfo=', '2026-03-03 15:16:09.368934', null)
DROP TABLE IF EXISTS task_user;
DROP TABLE IF EXISTS project_user;
DROP TABLE IF EXISTS comments;
DROP TABLE IF EXISTS projects CASCADE;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS tasks;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS log;

DROP SEQUENCE IF EXISTS users_id_seq;
DROP SEQUENCE IF EXISTS comments_id_seq;
DROP SEQUENCE IF EXISTS tasks_id_seq;
DROP SEQUENCE IF EXISTS projects_id_seq;
DROP SEQUENCE IF EXISTS log_id_seq;

CREATE SEQUENCE users_id_seq START WITH 1;
CREATE SEQUENCE comments_id_seq START WITH 1;
CREATE SEQUENCE tasks_id_seq START WITH 1;
CREATE SEQUENCE projects_id_seq START WITH 1;
CREATE SEQUENCE log_id_seq START WITH 1;


CREATE TABLE roles(
    id INT PRIMARY KEY,
    name VARCHAR(16) NOT NULL
);

INSERT INTO roles(id, name) VALUES (1, "ROLE_ADMIN"), (2, "ROLE_USER"), (3, "ROLE_GUEST");

CREATE TABLE users(
    id BIGINT PRIMARY KEY DEFAULT nextval('users_id_seq'),
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    birth_date DATE,
    info VARCHAR(512),
    full_name VARCHAR(128),
    role_id INT NOT NULL DEFAULT 2
);

ALTER TABLE users ADD CONSTRAINT users_role_id_fk
FOREIGN KEY(role_id) REFERENCES roles(id);

CREATE TABLE projects(
    id BIGINT PRIMARY KEY DEFAULT nextval('projects_id_seq'),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    is_private BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT NOW(),
    creator_id INT
);

ALTER TABLE projects ADD CONSTRAINT projects_creator_id_fk
FOREIGN KEY(creator_id) REFERENCES users(id)
ON DELETE SET DEFAULT;


CREATE TABLE tasks(
    id BIGINT PRIMARY KEY DEFAULT nextval('tasks_id_seq'),
    title VARCHAR(128) NOT NULL,
    content TEXT,
    creation_date TIMESTAMP DEFAULT NOW(),
    expiration_date TIMESTAMP,
    status INT DEFAULT 0,
    execution_time TIMESTAMP,
    project_id BIGINT NOT NULL
);

ALTER TABLE tasks ADD CONSTRAINT tasks_project_id_fk 
FOREIGN KEY(project_id) REFERENCES projects(id)
ON DELETE CASCADE;

CREATE TABLE comments(
    id BIGINT PRIMARY KEY DEFAULT nextval('comments_id_seq'),
    content VARCHAR(512) NOT NULL,
    creation_date TIMESTAMP DEFAULT NOW(),
    user_id BIGINT,
    task_id BIGINT NOT NULL
);

ALTER TABLE comments ADD CONSTRAINT comments_user_id_fk
FOREIGN KEY(user_id) REFERENCES users(id)
ON DELETE SET DEFAULT;

ALTER TABLE comments ADD CONSTRAINT comments_task_id_fk
FOREIGN KEY(task_id) REFERENCES tasks(id)
ON DELETE CASCADE;
 
CREATE TABLE task_user(
    task_id BIGINT NOT NULL,
    user_id BIGINT NOT NULL
);

ALTER TABLE task_user ADD CONSTRAINT task_user_task_id_fk
FOREIGN KEY(task_id) REFERENCES tasks(id)
ON DELETE CASCADE;

ALTER TABLE task_user ADD CONSTRAINT task_user_user_id_fk 
FOREIGN KEY(user_id) REFERENCES users(id)
ON DELETE CASCADE;

CREATE TABLE project_user(
    project_id INT NOT NULL,
    user_id INT NOT NULL
);

ALTER TABLE project_user ADD CONSTRAINT project_user_user_id_fk 
FOREIGN KEY(user_id) REFERENCES users(id)
ON DELETE CASCADE;

ALTER TABLE project_user ADD CONSTRAINT project_user_project_id_fk
FOREIGN KEY(project_id) REFERENCES projects(id)
ON DELETE CASCADE;

CREATE TABLE log(
    id BIGINT PRIMARY KEY DEFAULT nextval('log_id_seq'),
    date_created TIMESTAMP NOT NULL,
    thread VARCHAR(255),
    level VARCHAR(50),
    logger VARCHAR(255),
    message VARCHAR(4096),
    exception VARCHAR(2048)
);

INSERT INTO users(email, birth_date, info, full_name, password, role_id) VALUES 
('1@mail.ru', now() - interval '20 years', 'first info', 'First Second Third', 'p03i12kjfdsf', 1),
('2@gmail.com', now() - interval '15 years', 'second info', 'First Second Third', 'p03i12kjfdsf', 2),
('3@yandex.ru', now() - interval '14 years', 'third info', 'First Second Third', 'p03i12kjfdsf', 2),
('4@mail.ru', now() - interval '58 years', 'fourth info', 'First Second Third', 'p03i12kjfdsf', 2),
('5@yahoo.com', now() - interval '24 years', 'fifth info', 'First Second Third', 'p03i12kjfdsf', 2);


INSERT INTO projects(name, description, is_private, created_at, creator_id) VALUES
('chat', 'it is chat', FALSE, DEFAULT, 1),
('pm', 'it is somethign', FALSE, DEFAULT, 1),
('music player', 'it is not chat', FALSE, DEFAULT, 1),
('todo list', 'you can craete tasks', FALSE, DEFAULT, 4),
('something', 'it is not something', FALSE, DEFAULT, 2),
('i will regret it', 'yes', FALSE, DEFAULT, 3);

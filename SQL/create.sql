DROP TABLE IF EXISTS task_user;
DROP TABLE IF EXISTS project_user;
DROP TABLE IF EXISTS comments;
DROP TABLE IF EXISTS projects CASCADE;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS tasks;
DROP TABLE IF EXISTS rights;

DROP SEQUENCE IF EXISTS users_id_seq;
DROP SEQUENCE IF EXISTS comments_id_seq;
DROP SEQUENCE IF EXISTS tasks_id_seq;
DROP SEQUENCE IF EXISTS projects_id_seq;
DROP SEQUENCE IF EXISTS rights_id_seq;

CREATE SEQUENCE users_id_seq START WITH 1;
CREATE SEQUENCE comments_id_seq START WITH 1;
CREATE SEQUENCE tasks_id_seq START WITH 1;
CREATE SEQUENCE projects_id_seq START WITH 1;
CREATE SEQUENCE rights_id_seq START WITH 1;

CREATE TABLE rights(
    id BIGINT PRIMARY KEY DEFAULT nextval('rights_id_seq'),
    name VARCHAR(16) NOT NULL
);

CREATE TABLE users(
    id BIGINT PRIMARY KEY DEFAULT nextval('users_id_seq'),
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    birth_date DATE,
    info VARCHAR(512),
    full_name VARCHAR(128),
    rights_id INT
);

ALTER TABLE users ADD CONSTRAINT users_rigths_id_fk
FOREIGN KEY(rights_id) REFERENCES rights(id)
ON DELETE SET DEFAULT;

CREATE TABLE projects(
    id BIGINT PRIMARY KEY DEFAULT nextval('projects_id_seq'),
    name VARCHAR(255) NOT NULL,
    descripton TEXT,
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

INSERT INTO rights(id, name) VALUES 
(1, 'admin'), (2, 'user'), (3, 'guest');

INSERT INTO users(email, birth_date, info, full_name, rights_id, password) VALUES 
('1@mail.ru', now() - interval '20 years', 'first info', 'First Second Third', 1, 'p03i12kjfdsf'),
('2@gmail.com', now() - interval '15 years', 'second info', 'First Second Third', 2, 'p03i12kjfdsf'),
('3@yandex.ru', now() - interval '14 years', 'third info', 'First Second Third', 2, 'p03i12kjfdsf'),
('4@mail.ru', now() - interval '58 years', 'fourth info', 'First Second Third', 2, 'p03i12kjfdsf'),
('5@yahoo.com', now() - interval '24 years', 'fifth info', 'First Second Third', 1, 'p03i12kjfdsf');

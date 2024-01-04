DROP TABLE IF EXISTS pairs;

CREATE TABLE pairs (
	id serial PRIMARY KEY,
	key varchar(255) UNIQUE NOT NULL,
	value text,
	expires_at timestamp DEFAULT NOW(),
	expiration_period interval NOT NULL
);

CREATE INDEX key_index ON pairs(key);
CREATE INDEX expires_at_index ON pairs(expires_at);
--
-- PostgreSQL database dump
--

-- Dumped from database version 9.4.5
-- Dumped by pg_dump version 9.4.0
-- Started on 2015-12-26 22:29:31

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- TOC entry 178 (class 3079 OID 11855)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2040 (class 0 OID 0)
-- Dependencies: 178
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 173 (class 1259 OID 16399)
-- Name: channels; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE channels (
    channelid integer NOT NULL,
    youtubeid character(30) NOT NULL,
    title character(150) NOT NULL,
    description character(1000),
    uploadplaylist character(30),
    thumbnail character(100),
    viewcount integer,
    subscribercount integer,
    videocount integer
);


ALTER TABLE channels OWNER TO postgres;

--
-- TOC entry 175 (class 1259 OID 16417)
-- Name: collections; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE collections (
    collectionid integer NOT NULL,
    userid integer NOT NULL,
    title character(50) NOT NULL,
    description character(150)
);


ALTER TABLE collections OWNER TO postgres;

--
-- TOC entry 176 (class 1259 OID 16427)
-- Name: collections_channels; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE collections_channels (
    collectionchannelid integer NOT NULL,
    collectionid integer NOT NULL,
    channelid integer NOT NULL
);


ALTER TABLE collections_channels OWNER TO postgres;

--
-- TOC entry 172 (class 1259 OID 16394)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE users (
    userid integer NOT NULL,
    username character(50)
);


ALTER TABLE users OWNER TO postgres;

--
-- TOC entry 177 (class 1259 OID 16442)
-- Name: users_videos; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE users_videos (
    uservideoid integer NOT NULL,
    userid integer NOT NULL,
    videoid integer NOT NULL,
    dateviewed character(15)
);


ALTER TABLE users_videos OWNER TO postgres;

--
-- TOC entry 174 (class 1259 OID 16407)
-- Name: videos; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE videos (
    videoid integer NOT NULL,
    youtubeid character(15) NOT NULL,
    channelid integer NOT NULL,
    title character(150) NOT NULL,
    thumbnail character(100),
    duration character(15),
    viewcount integer
);


ALTER TABLE videos OWNER TO postgres;

--
-- TOC entry 2028 (class 0 OID 16399)
-- Dependencies: 173
-- Data for Name: channels; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY channels (channelid, youtubeid, title, description, uploadplaylist, thumbnail, viewcount, subscribercount, videocount) FROM stdin;
\.


--
-- TOC entry 2030 (class 0 OID 16417)
-- Dependencies: 175
-- Data for Name: collections; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY collections (collectionid, userid, title, description) FROM stdin;
\.


--
-- TOC entry 2031 (class 0 OID 16427)
-- Dependencies: 176
-- Data for Name: collections_channels; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY collections_channels (collectionchannelid, collectionid, channelid) FROM stdin;
\.


--
-- TOC entry 2027 (class 0 OID 16394)
-- Dependencies: 172
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY users (userid, username) FROM stdin;
\.


--
-- TOC entry 2032 (class 0 OID 16442)
-- Dependencies: 177
-- Data for Name: users_videos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY users_videos (uservideoid, userid, videoid, dateviewed) FROM stdin;
\.


--
-- TOC entry 2029 (class 0 OID 16407)
-- Dependencies: 174
-- Data for Name: videos; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY videos (videoid, youtubeid, channelid, title, thumbnail, duration, viewcount) FROM stdin;
\.


--
-- TOC entry 1903 (class 2606 OID 16406)
-- Name: channels_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY channels
    ADD CONSTRAINT channels_pkey PRIMARY KEY (channelid);


--
-- TOC entry 1909 (class 2606 OID 16431)
-- Name: collections_channels_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY collections_channels
    ADD CONSTRAINT collections_channels_pkey PRIMARY KEY (collectionchannelid);


--
-- TOC entry 1907 (class 2606 OID 16421)
-- Name: collections_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY collections
    ADD CONSTRAINT collections_pkey PRIMARY KEY (collectionid);


--
-- TOC entry 1901 (class 2606 OID 16398)
-- Name: users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- TOC entry 1911 (class 2606 OID 16446)
-- Name: users_videos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY users_videos
    ADD CONSTRAINT users_videos_pkey PRIMARY KEY (uservideoid);


--
-- TOC entry 1905 (class 2606 OID 16411)
-- Name: videos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY videos
    ADD CONSTRAINT videos_pkey PRIMARY KEY (videoid);


--
-- TOC entry 1915 (class 2606 OID 16437)
-- Name: collections_channels_channelid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY collections_channels
    ADD CONSTRAINT collections_channels_channelid_fkey FOREIGN KEY (channelid) REFERENCES channels(channelid);


--
-- TOC entry 1914 (class 2606 OID 16432)
-- Name: collections_channels_collectionid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY collections_channels
    ADD CONSTRAINT collections_channels_collectionid_fkey FOREIGN KEY (collectionid) REFERENCES collections(collectionid);


--
-- TOC entry 1913 (class 2606 OID 16422)
-- Name: collections_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY collections
    ADD CONSTRAINT collections_userid_fkey FOREIGN KEY (userid) REFERENCES users(userid);


--
-- TOC entry 1916 (class 2606 OID 16447)
-- Name: users_videos_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_videos
    ADD CONSTRAINT users_videos_userid_fkey FOREIGN KEY (userid) REFERENCES users(userid);


--
-- TOC entry 1917 (class 2606 OID 16452)
-- Name: users_videos_videoid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY users_videos
    ADD CONSTRAINT users_videos_videoid_fkey FOREIGN KEY (videoid) REFERENCES videos(videoid);


--
-- TOC entry 1912 (class 2606 OID 16412)
-- Name: videos_channelid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY videos
    ADD CONSTRAINT videos_channelid_fkey FOREIGN KEY (channelid) REFERENCES channels(channelid);


--
-- TOC entry 2039 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2015-12-26 22:29:31

--
-- PostgreSQL database dump complete
--


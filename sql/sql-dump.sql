--
-- PostgreSQL database dump
--

-- Dumped from database version 10.11
-- Dumped by pg_dump version 10.11

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: turnit_store; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE turnit_store ENCODING = 'UTF8';


ALTER DATABASE turnit_store OWNER TO postgres;

\connect turnit_store

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: category; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.category (
    id uuid NOT NULL,
    name text NOT NULL
);


ALTER TABLE public.category OWNER TO postgres;

--
-- Name: product; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product (
    id uuid NOT NULL,
    name text NOT NULL,
    description text
);


ALTER TABLE public.product OWNER TO postgres;

--
-- Name: product_availability; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_availability (
    id uuid NOT NULL,
    product_id uuid NOT NULL,
    store_id uuid NOT NULL,
    availability integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.product_availability OWNER TO postgres;

--
-- Name: product_category; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_category (
    id uuid NOT NULL,
    product_id uuid NOT NULL,
    category_id uuid NOT NULL
);


ALTER TABLE public.product_category OWNER TO postgres;

--
-- Name: store; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.store (
    id uuid NOT NULL,
    name text NOT NULL
);


ALTER TABLE public.store OWNER TO postgres;

--
-- Data for Name: category; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.category (id, name) FROM stdin;
4f0f4a0e-e65b-11ec-a1ab-24ee9a88c06f	Electronics
4f10a98a-e65b-11ec-a1ac-24ee9a88c06f	Home
4f10a98b-e65b-11ec-a1ad-24ee9a88c06f	Movies
4f10a98c-e65b-11ec-a1ae-24ee9a88c06f	Car parts
4f10a98d-e65b-11ec-a1af-24ee9a88c06f	Game consoles
\.


--
-- Data for Name: product; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product (id, name, description) FROM stdin;
6f32650e-e65c-11ec-a1b2-24ee9a88c06f	Kitchen mixer	A great product for smoothies
6f32650f-e65c-11ec-a1b3-24ee9a88c06f	Kitchen table	Suitable up to 6 peaople
6f326510-e65c-11ec-a1b4-24ee9a88c06f	Terminator 2 DVD	Greatest movie ever
6f326511-e65c-11ec-a1b5-24ee9a88c06f	Avatar 4KD BluRay	A good watch
6f326512-e65c-11ec-a1b6-24ee9a88c06f	V8 engine	Proper for muscle car
6f326513-e65c-11ec-a1b7-24ee9a88c06f	PlayStation 4	A game console
6f326514-e65c-11ec-a1b8-24ee9a88c06f	Xbox One	A game console
6f326515-e65c-11ec-a1b9-24ee9a88c06f	Black T-Shirt	Size XL
\.


--
-- Data for Name: product_availability; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_availability (id, product_id, store_id, availability) FROM stdin;
23764e4a-e65d-11ec-a1c7-24ee9a88c06f	6f32650e-e65c-11ec-a1b2-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	43
23769c60-e65d-11ec-a1c8-24ee9a88c06f	6f32650f-e65c-11ec-a1b3-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	23
23769c61-e65d-11ec-a1c9-24ee9a88c06f	6f326510-e65c-11ec-a1b4-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	66
23769c62-e65d-11ec-a1ca-24ee9a88c06f	6f326511-e65c-11ec-a1b5-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	1
23769c63-e65d-11ec-a1cb-24ee9a88c06f	6f326512-e65c-11ec-a1b6-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	27
23769c64-e65d-11ec-a1cc-24ee9a88c06f	6f326513-e65c-11ec-a1b7-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	40
23769c65-e65d-11ec-a1cd-24ee9a88c06f	6f326514-e65c-11ec-a1b8-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	53
23769c66-e65d-11ec-a1ce-24ee9a88c06f	6f326515-e65c-11ec-a1b9-24ee9a88c06f	89119170-e65c-11ec-a1c4-24ee9a88c06f	89
23769c67-e65d-11ec-a1cf-24ee9a88c06f	6f32650e-e65c-11ec-a1b2-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	34
23769c68-e65d-11ec-a1d0-24ee9a88c06f	6f32650f-e65c-11ec-a1b3-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	55
23769c69-e65d-11ec-a1d1-24ee9a88c06f	6f326510-e65c-11ec-a1b4-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	96
23769c6a-e65d-11ec-a1d2-24ee9a88c06f	6f326511-e65c-11ec-a1b5-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	31
2376c35c-e65d-11ec-a1d3-24ee9a88c06f	6f326512-e65c-11ec-a1b6-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	98
2376c35d-e65d-11ec-a1d4-24ee9a88c06f	6f326513-e65c-11ec-a1b7-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	9
2376c35e-e65d-11ec-a1d5-24ee9a88c06f	6f326514-e65c-11ec-a1b8-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	17
2376c35f-e65d-11ec-a1d6-24ee9a88c06f	6f326515-e65c-11ec-a1b9-24ee9a88c06f	8911b86c-e65c-11ec-a1c5-24ee9a88c06f	69
2376c360-e65d-11ec-a1d7-24ee9a88c06f	6f32650e-e65c-11ec-a1b2-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	23
2376c361-e65d-11ec-a1d8-24ee9a88c06f	6f32650f-e65c-11ec-a1b3-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	47
2376c362-e65d-11ec-a1d9-24ee9a88c06f	6f326510-e65c-11ec-a1b4-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	68
2376c363-e65d-11ec-a1da-24ee9a88c06f	6f326511-e65c-11ec-a1b5-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	27
2376c364-e65d-11ec-a1db-24ee9a88c06f	6f326512-e65c-11ec-a1b6-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	99
2376c365-e65d-11ec-a1dc-24ee9a88c06f	6f326513-e65c-11ec-a1b7-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	35
2376c366-e65d-11ec-a1dd-24ee9a88c06f	6f326514-e65c-11ec-a1b8-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	8
2376ea80-e65d-11ec-a1de-24ee9a88c06f	6f326515-e65c-11ec-a1b9-24ee9a88c06f	8911b86d-e65c-11ec-a1c6-24ee9a88c06f	64
\.


--
-- Data for Name: product_category; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_category (id, product_id, category_id) FROM stdin;
706acaba-e65c-11ec-a1ba-24ee9a88c06f	6f32650e-e65c-11ec-a1b2-24ee9a88c06f	4f0f4a0e-e65b-11ec-a1ab-24ee9a88c06f
706b18bc-e65c-11ec-a1bb-24ee9a88c06f	6f326513-e65c-11ec-a1b7-24ee9a88c06f	4f0f4a0e-e65b-11ec-a1ab-24ee9a88c06f
706b18bd-e65c-11ec-a1bc-24ee9a88c06f	6f326514-e65c-11ec-a1b8-24ee9a88c06f	4f0f4a0e-e65b-11ec-a1ab-24ee9a88c06f
706b18be-e65c-11ec-a1bd-24ee9a88c06f	6f32650e-e65c-11ec-a1b2-24ee9a88c06f	4f10a98a-e65b-11ec-a1ac-24ee9a88c06f
706b18bf-e65c-11ec-a1be-24ee9a88c06f	6f32650f-e65c-11ec-a1b3-24ee9a88c06f	4f10a98a-e65b-11ec-a1ac-24ee9a88c06f
706b18c0-e65c-11ec-a1bf-24ee9a88c06f	6f326510-e65c-11ec-a1b4-24ee9a88c06f	4f10a98b-e65b-11ec-a1ad-24ee9a88c06f
706b18c1-e65c-11ec-a1c0-24ee9a88c06f	6f326511-e65c-11ec-a1b5-24ee9a88c06f	4f10a98b-e65b-11ec-a1ad-24ee9a88c06f
706b18c2-e65c-11ec-a1c1-24ee9a88c06f	6f326512-e65c-11ec-a1b6-24ee9a88c06f	4f10a98c-e65b-11ec-a1ae-24ee9a88c06f
706b18c3-e65c-11ec-a1c2-24ee9a88c06f	6f326513-e65c-11ec-a1b7-24ee9a88c06f	4f10a98d-e65b-11ec-a1af-24ee9a88c06f
706b18c4-e65c-11ec-a1c3-24ee9a88c06f	6f326514-e65c-11ec-a1b8-24ee9a88c06f	4f10a98d-e65b-11ec-a1af-24ee9a88c06f
\.


--
-- Data for Name: store; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.store (id, name) FROM stdin;
89119170-e65c-11ec-a1c4-24ee9a88c06f	Asia warehouse
8911b86c-e65c-11ec-a1c5-24ee9a88c06f	Europe warehouse
8911b86d-e65c-11ec-a1c6-24ee9a88c06f	US West warehouse
\.


--
-- Name: category category_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_pk PRIMARY KEY (id);


--
-- Name: product_availability product_availability_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_availability
    ADD CONSTRAINT product_availability_pk PRIMARY KEY (id);


--
-- Name: product_category product_category_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_category
    ADD CONSTRAINT product_category_pk PRIMARY KEY (id);


--
-- Name: product product_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_pk PRIMARY KEY (id);


--
-- Name: store store_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.store
    ADD CONSTRAINT store_pk PRIMARY KEY (id);


--
-- Name: category_id_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX category_id_uindex ON public.category USING btree (id);


--
-- Name: product_availability_id_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX product_availability_id_uindex ON public.product_availability USING btree (id);


--
-- Name: product_availability_product_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_availability_product_id_index ON public.product_availability USING btree (product_id);


--
-- Name: product_availability_store_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_availability_store_id_index ON public.product_availability USING btree (store_id);


--
-- Name: product_category_category_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_category_category_id_index ON public.product_category USING btree (category_id);


--
-- Name: product_category_id_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX product_category_id_uindex ON public.product_category USING btree (id);


--
-- Name: product_category_product_id_index; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX product_category_product_id_index ON public.product_category USING btree (product_id);


--
-- Name: product_id_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX product_id_uindex ON public.product USING btree (id);


--
-- Name: store_id_uindex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX store_id_uindex ON public.store USING btree (id);


--
-- Name: product_availability product_availability_product_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_availability
    ADD CONSTRAINT product_availability_product_id_fk FOREIGN KEY (product_id) REFERENCES public.product(id);


--
-- Name: product_availability product_availability_store_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_availability
    ADD CONSTRAINT product_availability_store_id_fk FOREIGN KEY (store_id) REFERENCES public.store(id);


--
-- Name: product_category product_category_category_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_category
    ADD CONSTRAINT product_category_category_id_fk FOREIGN KEY (category_id) REFERENCES public.category(id);


--
-- Name: product_category product_category_product_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_category
    ADD CONSTRAINT product_category_product_id_fk FOREIGN KEY (product_id) REFERENCES public.product(id);


--
-- PostgreSQL database dump complete
--


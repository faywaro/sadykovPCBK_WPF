--
-- PostgreSQL database dump
--

\restrict LEPh4J2HZVem39thJOIwiO5yRsU54U9pzcjnaph7U3igFuNswsueMsrcer8fCTk

-- Dumped from database version 16.10
-- Dumped by pg_dump version 16.10

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
-- Name: app; Type: SCHEMA; Schema: -; Owner: app
--

CREATE SCHEMA app;


ALTER SCHEMA app OWNER TO app;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: partner_sales; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.partner_sales (
    id integer NOT NULL,
    partner_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    sale_date date DEFAULT CURRENT_DATE NOT NULL,
    CONSTRAINT partner_sales_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE app.partner_sales OWNER TO postgres;

--
-- Name: partner_sales_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.partner_sales_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partner_sales_id_seq OWNER TO postgres;

--
-- Name: partner_sales_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.partner_sales_id_seq OWNED BY app.partner_sales.id;


--
-- Name: partner_types; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.partner_types (
    id integer NOT NULL,
    type_name character varying(100) NOT NULL
);


ALTER TABLE app.partner_types OWNER TO postgres;

--
-- Name: partner_types_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.partner_types_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partner_types_id_seq OWNER TO postgres;

--
-- Name: partner_types_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.partner_types_id_seq OWNED BY app.partner_types.id;


--
-- Name: partners; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.partners (
    id integer NOT NULL,
    type_id integer NOT NULL,
    company_name character varying(255) NOT NULL,
    legal_address character varying(500) NOT NULL,
    inn character varying(12) NOT NULL,
    director_name character varying(255) NOT NULL,
    phone character varying(20) NOT NULL,
    email character varying(255) NOT NULL,
    rating integer DEFAULT 0 NOT NULL,
    logo_path character varying(500),
    CONSTRAINT partners_rating_check CHECK ((rating >= 0))
);


ALTER TABLE app.partners OWNER TO postgres;

--
-- Name: partners_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.partners_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partners_id_seq OWNER TO postgres;

--
-- Name: partners_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.partners_id_seq OWNED BY app.partners.id;


--
-- Name: products; Type: TABLE; Schema: app; Owner: postgres
--

CREATE TABLE app.products (
    id integer NOT NULL,
    article character varying(50) NOT NULL,
    product_name character varying(255) NOT NULL,
    product_type character varying(100) NOT NULL,
    min_price numeric(12,2) NOT NULL,
    CONSTRAINT products_min_price_check CHECK ((min_price >= (0)::numeric))
);


ALTER TABLE app.products OWNER TO postgres;

--
-- Name: products_id_seq; Type: SEQUENCE; Schema: app; Owner: postgres
--

CREATE SEQUENCE app.products_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.products_id_seq OWNER TO postgres;

--
-- Name: products_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: postgres
--

ALTER SEQUENCE app.products_id_seq OWNED BY app.products.id;


--
-- Name: partner_sales id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales ALTER COLUMN id SET DEFAULT nextval('app.partner_sales_id_seq'::regclass);


--
-- Name: partner_types id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_types ALTER COLUMN id SET DEFAULT nextval('app.partner_types_id_seq'::regclass);


--
-- Name: partners id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners ALTER COLUMN id SET DEFAULT nextval('app.partners_id_seq'::regclass);


--
-- Name: products id; Type: DEFAULT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.products ALTER COLUMN id SET DEFAULT nextval('app.products_id_seq'::regclass);


--
-- Data for Name: partner_sales; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.partner_sales (id, partner_id, product_id, quantity, sale_date) FROM stdin;
1	1	1	85000	2023-03-15
2	1	2	40000	2023-07-20
3	1	7	60000	2023-11-05
4	1	8	30000	2024-02-14
5	1	3	55000	2024-05-10
6	2	5	20000	2023-04-12
7	2	6	15000	2023-09-18
8	2	9	10000	2024-01-22
9	3	1	120000	2023-01-10
10	3	2	80000	2023-06-25
11	3	3	70000	2023-10-30
12	3	7	90000	2024-03-18
13	3	8	50000	2024-07-04
14	4	5	8000	2023-05-20
15	4	12	2000	2024-02-28
16	5	1	35000	2023-02-14
17	5	7	25000	2023-08-09
18	5	11	5000	2024-04-17
19	6	6	18000	2023-06-01
20	6	8	12000	2023-12-15
21	6	9	9000	2024-06-20
22	7	10	4000	2023-07-11
23	7	12	1500	2024-01-05
24	8	5	22000	2023-09-03
25	8	4	16000	2024-03-27
26	8	10	8000	2024-08-14
27	1	1	85000	2023-03-15
28	1	2	40000	2023-07-20
29	1	7	60000	2023-11-05
30	1	8	30000	2024-02-14
31	1	3	55000	2024-05-10
32	2	5	20000	2023-04-12
33	2	6	15000	2023-09-18
34	2	9	10000	2024-01-22
35	3	1	120000	2023-01-10
36	3	2	80000	2023-06-25
37	3	3	70000	2023-10-30
38	3	7	90000	2024-03-18
39	3	8	50000	2024-07-04
40	4	5	8000	2023-05-20
41	4	12	2000	2024-02-28
42	5	1	35000	2023-02-14
43	5	7	25000	2023-08-09
44	5	11	5000	2024-04-17
45	6	6	18000	2023-06-01
46	6	8	12000	2023-12-15
47	6	9	9000	2024-06-20
48	7	10	4000	2023-07-11
49	7	12	1500	2024-01-05
50	8	5	22000	2023-09-03
51	8	4	16000	2024-03-27
52	8	10	8000	2024-08-14
53	17	3	12	2026-03-16
\.


--
-- Data for Name: partner_types; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.partner_types (id, type_name) FROM stdin;
1	ЗАО
2	ООО
3	ПАО
4	ОАО
5	АО
\.


--
-- Data for Name: partners; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.partners (id, type_id, company_name, legal_address, inn, director_name, phone, email, rating, logo_path) FROM stdin;
1	2	УралБумТорг	г. Екатеринбург, ул. Малышева, 36	6670123456	Власов Сергей Николаевич	+7 343 220 11 22	info@uralbumtorg.ru	9	\N
2	2	ПромУпаковка	г. Пермь, ул. Ленина, 58	5902234567	Кузнецов Алексей Владимирович	+7 342 270 33 44	order@promupak.ru	7	\N
3	5	КартонПлюс	г. Казань, пр. Победы, 14	1655345678	Фаттахов Марат Рустамович	+7 843 290 55 66	sales@kartonplus.ru	10	\N
4	1	СибПакет	г. Новосибирск, ул. Кирова, 29	5407456789	Морозов Дмитрий Александрович	+7 383 310 77 88	sibpaket@mail.ru	5	\N
5	2	ТД Гофра	г. Самара, ул. Советской Армии, 181	6315567890	Степанов Павел Игоревич	+7 846 330 99 00	gofra@td-gofra.ru	8	\N
6	3	РосУпак	г. Москва, Варшавское шоссе, 46	7736678901	Беляев Игорь Константинович	+7 495 640 12 34	contact@rosupak.ru	6	\N
7	2	ПакПром Урал	г. Челябинск, ул. Труда, 87	7451789012	Григорьев Виктор Семёнович	+7 351 260 56 78	pakprom@ural.ru	4	\N
8	4	Экопак	г. Нижний Новгород, ул. Горького, 117	5262890123	Романова Елена Сергеевна	+7 831 430 34 56	info@ekopak-nn.ru	7	\N
17	5	11111	11111	111111111111	11111111111111111	11111111111	111111111@mail.ru	111111	\N
\.


--
-- Data for Name: products; Type: TABLE DATA; Schema: app; Owner: postgres
--

COPY app.products (id, article, product_name, product_type, min_price) FROM stdin;
1	PCBK-001	Гофрокартон трёхслойный Т-23	Гофрокартон	18.50
2	PCBK-002	Гофрокартон пятислойный Т-24	Гофрокартон	26.00
3	PCBK-003	Картон тарный марки КТ-1	Картон	22.00
4	PCBK-004	Картон тарный марки КТ-2	Картон	19.80
5	PCBK-005	Бумага для гофрирования БГ-100	Бумага	14.30
6	PCBK-006	Бумага крафт мешочная 78 г/м	Бумага	32.00
7	PCBK-007	Коробка гофрированная 400x300x200	Упаковка	28.50
8	PCBK-008	Коробка гофрированная 600x400x300	Упаковка	42.00
9	PCBK-009	Коробка архивная А4 330x245x310	Упаковка	35.00
10	PCBK-010	Лоток гофрированный под продукты	Упаковка	12.00
11	PCBK-011	Поддон бумажный 1200x800	Упаковка	85.00
12	PCBK-012	Бумага офисная А4 80 г/м 500 л	Бумага	280.00
\.


--
-- Name: partner_sales_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.partner_sales_id_seq', 53, true);


--
-- Name: partner_types_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.partner_types_id_seq', 10, true);


--
-- Name: partners_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.partners_id_seq', 17, true);


--
-- Name: products_id_seq; Type: SEQUENCE SET; Schema: app; Owner: postgres
--

SELECT pg_catalog.setval('app.products_id_seq', 24, true);


--
-- Name: partner_sales partner_sales_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales
    ADD CONSTRAINT partner_sales_pkey PRIMARY KEY (id);


--
-- Name: partner_types partner_types_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_types
    ADD CONSTRAINT partner_types_pkey PRIMARY KEY (id);


--
-- Name: partner_types partner_types_type_name_key; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_types
    ADD CONSTRAINT partner_types_type_name_key UNIQUE (type_name);


--
-- Name: partners partners_inn_key; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT partners_inn_key UNIQUE (inn);


--
-- Name: partners partners_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT partners_pkey PRIMARY KEY (id);


--
-- Name: products products_article_key; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.products
    ADD CONSTRAINT products_article_key UNIQUE (article);


--
-- Name: products products_pkey; Type: CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (id);


--
-- Name: partner_sales partner_sales_partner_id_fkey; Type: FK CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales
    ADD CONSTRAINT partner_sales_partner_id_fkey FOREIGN KEY (partner_id) REFERENCES app.partners(id) ON DELETE CASCADE;


--
-- Name: partner_sales partner_sales_product_id_fkey; Type: FK CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partner_sales
    ADD CONSTRAINT partner_sales_product_id_fkey FOREIGN KEY (product_id) REFERENCES app.products(id) ON DELETE RESTRICT;


--
-- Name: partners partners_type_id_fkey; Type: FK CONSTRAINT; Schema: app; Owner: postgres
--

ALTER TABLE ONLY app.partners
    ADD CONSTRAINT partners_type_id_fkey FOREIGN KEY (type_id) REFERENCES app.partner_types(id) ON DELETE RESTRICT;


--
-- Name: TABLE partner_sales; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.partner_sales TO app;


--
-- Name: SEQUENCE partner_sales_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.partner_sales_id_seq TO app;


--
-- Name: TABLE partner_types; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.partner_types TO app;


--
-- Name: SEQUENCE partner_types_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.partner_types_id_seq TO app;


--
-- Name: TABLE partners; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.partners TO app;


--
-- Name: SEQUENCE partners_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.partners_id_seq TO app;


--
-- Name: TABLE products; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON TABLE app.products TO app;


--
-- Name: SEQUENCE products_id_seq; Type: ACL; Schema: app; Owner: postgres
--

GRANT ALL ON SEQUENCE app.products_id_seq TO app;


--
-- PostgreSQL database dump complete
--

\unrestrict LEPh4J2HZVem39thJOIwiO5yRsU54U9pzcjnaph7U3igFuNswsueMsrcer8fCTk


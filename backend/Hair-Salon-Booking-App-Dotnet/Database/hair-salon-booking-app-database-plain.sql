--
-- PostgreSQL database dump
--

-- Dumped from database version 16.4
-- Dumped by pg_dump version 16.4

-- Started on 2025-03-16 21:22:30

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
-- TOC entry 2 (class 3079 OID 16389)
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- TOC entry 3458 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


--
-- TOC entry 860 (class 1247 OID 16401)
-- Name: reservation_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.reservation_status AS ENUM (
    'PENDING',
    'COMPLETED',
    'CANCELLED'
);


ALTER TYPE public.reservation_status OWNER TO postgres;

--
-- TOC entry 235 (class 1255 OID 16407)
-- Name: generate_and_insert_working_hours(integer, time without time zone, time without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.generate_and_insert_working_hours(p_employee_id integer, p_start_time time without time zone, p_end_time time without time zone) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Delete any existing rows for this employee to avoid duplication
    DELETE FROM available_hours WHERE employee_id = p_employee_id;

    -- Generate the time slots and insert them into the available_hours table
    INSERT INTO available_hours (employee_id, start_time, end_time)
    SELECT 
        p_employee_id,
        (series::time) AS start_time,
        (series + interval '15 minutes')::time AS end_time
    FROM 
        generate_series(
            '2000-01-01'::timestamp + p_start_time::interval,  -- start as timestamp
            '2000-01-01'::timestamp + p_end_time::interval - interval '15 minutes',  -- end as timestamp
            interval '15 minutes'  -- step size
        ) AS series;
END;
$$;


ALTER FUNCTION public.generate_and_insert_working_hours(p_employee_id integer, p_start_time time without time zone, p_end_time time without time zone) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 216 (class 1259 OID 16408)
-- Name: available_hours; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.available_hours (
    id integer NOT NULL,
    employee_id integer,
    start_time time without time zone,
    end_time time without time zone
);


ALTER TABLE public.available_hours OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16411)
-- Name: available_hours_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.available_hours_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.available_hours_id_seq OWNER TO postgres;

--
-- TOC entry 3459 (class 0 OID 0)
-- Dependencies: 217
-- Name: available_hours_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.available_hours_id_seq OWNED BY public.available_hours.id;


--
-- TOC entry 218 (class 1259 OID 16412)
-- Name: employees; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employees (
    employee_id integer NOT NULL,
    first_name character varying(255) NOT NULL,
    last_name character varying(255) NOT NULL
);


ALTER TABLE public.employees OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16417)
-- Name: employees_employee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.employees ALTER COLUMN employee_id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.employees_employee_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 220 (class 1259 OID 16418)
-- Name: reservations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.reservations (
    id integer NOT NULL,
    user_id character varying(255) NOT NULL,
    reservation_date date,
    end_time time without time zone NOT NULL,
    status character varying(255) DEFAULT 'PENDING'::public.reservation_status NOT NULL,
    service_id integer,
    employee_id integer NOT NULL,
    start_time time without time zone
);


ALTER TABLE public.reservations OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16424)
-- Name: reservations_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.reservations_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.reservations_id_seq OWNER TO postgres;

--
-- TOC entry 3460 (class 0 OID 0)
-- Dependencies: 221
-- Name: reservations_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.reservations_id_seq OWNED BY public.reservations.id;


--
-- TOC entry 222 (class 1259 OID 16425)
-- Name: services; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.services (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    duration time without time zone,
    price numeric(38,2) NOT NULL
);


ALTER TABLE public.services OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16428)
-- Name: services_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.services ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.services_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 224 (class 1259 OID 16429)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    username character varying(255) NOT NULL,
    password character varying(255) NOT NULL,
    role character varying(255) DEFAULT 'user'::character varying NOT NULL,
    refresh_token character varying(255),
    CONSTRAINT users_role_check CHECK (((role)::text = ANY (ARRAY[('user'::character varying)::text, ('admin'::character varying)::text, ('employee'::character varying)::text])))
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 3280 (class 2604 OID 16437)
-- Name: available_hours id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.available_hours ALTER COLUMN id SET DEFAULT nextval('public.available_hours_id_seq'::regclass);


--
-- TOC entry 3281 (class 2604 OID 16438)
-- Name: reservations id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations ALTER COLUMN id SET DEFAULT nextval('public.reservations_id_seq'::regclass);


--
-- TOC entry 3444 (class 0 OID 16408)
-- Dependencies: 216
-- Data for Name: available_hours; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.available_hours (id, employee_id, start_time, end_time) FROM stdin;
1	1	09:00:00	09:15:00
2	1	09:15:00	09:30:00
3	1	09:30:00	09:45:00
4	1	09:45:00	10:00:00
5	1	10:00:00	10:15:00
6	1	10:15:00	10:30:00
7	1	10:30:00	10:45:00
8	1	10:45:00	11:00:00
9	1	11:00:00	11:15:00
10	1	11:15:00	11:30:00
11	1	11:30:00	11:45:00
12	1	11:45:00	12:00:00
13	1	12:00:00	12:15:00
14	1	12:15:00	12:30:00
15	1	12:30:00	12:45:00
16	1	12:45:00	13:00:00
17	1	13:00:00	13:15:00
18	1	13:15:00	13:30:00
19	1	13:30:00	13:45:00
20	1	13:45:00	14:00:00
21	1	14:00:00	14:15:00
22	1	14:15:00	14:30:00
23	1	14:30:00	14:45:00
24	1	14:45:00	15:00:00
25	1	15:00:00	15:15:00
26	1	15:15:00	15:30:00
27	1	15:30:00	15:45:00
28	1	15:45:00	16:00:00
29	1	16:00:00	16:15:00
30	1	16:15:00	16:30:00
31	1	16:30:00	16:45:00
32	1	16:45:00	17:00:00
33	2	09:00:00	09:15:00
34	2	09:15:00	09:30:00
35	2	09:30:00	09:45:00
36	2	09:45:00	10:00:00
37	2	10:00:00	10:15:00
38	2	10:15:00	10:30:00
39	2	10:30:00	10:45:00
40	2	10:45:00	11:00:00
41	2	11:00:00	11:15:00
42	2	11:15:00	11:30:00
43	2	11:30:00	11:45:00
44	2	11:45:00	12:00:00
45	2	12:00:00	12:15:00
46	2	12:15:00	12:30:00
47	2	12:30:00	12:45:00
48	2	12:45:00	13:00:00
49	2	13:00:00	13:15:00
50	2	13:15:00	13:30:00
51	2	13:30:00	13:45:00
52	2	13:45:00	14:00:00
53	2	14:00:00	14:15:00
54	2	14:15:00	14:30:00
55	2	14:30:00	14:45:00
56	2	14:45:00	15:00:00
57	2	15:00:00	15:15:00
58	2	15:15:00	15:30:00
59	2	15:30:00	15:45:00
60	2	15:45:00	16:00:00
61	2	16:00:00	16:15:00
62	2	16:15:00	16:30:00
63	2	16:30:00	16:45:00
64	2	16:45:00	17:00:00
65	3	09:00:00	09:15:00
66	3	09:15:00	09:30:00
67	3	09:30:00	09:45:00
68	3	09:45:00	10:00:00
69	3	10:00:00	10:15:00
70	3	10:15:00	10:30:00
71	3	10:30:00	10:45:00
72	3	10:45:00	11:00:00
73	3	11:00:00	11:15:00
74	3	11:15:00	11:30:00
75	3	11:30:00	11:45:00
76	3	11:45:00	12:00:00
77	3	12:00:00	12:15:00
78	3	12:15:00	12:30:00
79	3	12:30:00	12:45:00
80	3	12:45:00	13:00:00
81	3	13:00:00	13:15:00
82	3	13:15:00	13:30:00
83	3	13:30:00	13:45:00
84	3	13:45:00	14:00:00
85	3	14:00:00	14:15:00
86	3	14:15:00	14:30:00
87	3	14:30:00	14:45:00
88	3	14:45:00	15:00:00
89	3	15:00:00	15:15:00
90	3	15:15:00	15:30:00
91	3	15:30:00	15:45:00
92	3	15:45:00	16:00:00
93	3	16:00:00	16:15:00
94	3	16:15:00	16:30:00
95	3	16:30:00	16:45:00
96	3	16:45:00	17:00:00
97	4	09:00:00	09:15:00
98	4	09:15:00	09:30:00
99	4	09:30:00	09:45:00
100	4	09:45:00	10:00:00
101	4	10:00:00	10:15:00
102	4	10:15:00	10:30:00
103	4	10:30:00	10:45:00
104	4	10:45:00	11:00:00
105	4	11:00:00	11:15:00
106	4	11:15:00	11:30:00
107	4	11:30:00	11:45:00
108	4	11:45:00	12:00:00
109	4	12:00:00	12:15:00
110	4	12:15:00	12:30:00
111	4	12:30:00	12:45:00
112	4	12:45:00	13:00:00
113	4	13:00:00	13:15:00
114	4	13:15:00	13:30:00
115	4	13:30:00	13:45:00
116	4	13:45:00	14:00:00
117	4	14:00:00	14:15:00
118	4	14:15:00	14:30:00
119	4	14:30:00	14:45:00
120	4	14:45:00	15:00:00
121	4	15:00:00	15:15:00
122	4	15:15:00	15:30:00
123	4	15:30:00	15:45:00
124	4	15:45:00	16:00:00
125	4	16:00:00	16:15:00
126	4	16:15:00	16:30:00
127	4	16:30:00	16:45:00
128	4	16:45:00	17:00:00
129	5	09:00:00	09:15:00
130	5	09:15:00	09:30:00
131	5	09:30:00	09:45:00
132	5	09:45:00	10:00:00
133	5	10:00:00	10:15:00
134	5	10:15:00	10:30:00
135	5	10:30:00	10:45:00
136	5	10:45:00	11:00:00
137	5	11:00:00	11:15:00
138	5	11:15:00	11:30:00
139	5	11:30:00	11:45:00
140	5	11:45:00	12:00:00
141	5	12:00:00	12:15:00
142	5	12:15:00	12:30:00
143	5	12:30:00	12:45:00
144	5	12:45:00	13:00:00
145	5	13:00:00	13:15:00
146	5	13:15:00	13:30:00
147	5	13:30:00	13:45:00
148	5	13:45:00	14:00:00
149	5	14:00:00	14:15:00
150	5	14:15:00	14:30:00
151	5	14:30:00	14:45:00
152	5	14:45:00	15:00:00
153	5	15:00:00	15:15:00
154	5	15:15:00	15:30:00
155	5	15:30:00	15:45:00
156	5	15:45:00	16:00:00
157	5	16:00:00	16:15:00
158	5	16:15:00	16:30:00
159	5	16:30:00	16:45:00
160	5	16:45:00	17:00:00
161	6	09:00:00	09:15:00
162	6	09:15:00	09:30:00
163	6	09:30:00	09:45:00
164	6	09:45:00	10:00:00
165	6	10:00:00	10:15:00
166	6	10:15:00	10:30:00
167	6	10:30:00	10:45:00
168	6	10:45:00	11:00:00
169	6	11:00:00	11:15:00
170	6	11:15:00	11:30:00
171	6	11:30:00	11:45:00
172	6	11:45:00	12:00:00
173	6	12:00:00	12:15:00
174	6	12:15:00	12:30:00
175	6	12:30:00	12:45:00
176	6	12:45:00	13:00:00
177	6	13:00:00	13:15:00
178	6	13:15:00	13:30:00
179	6	13:30:00	13:45:00
180	6	13:45:00	14:00:00
181	6	14:00:00	14:15:00
182	6	14:15:00	14:30:00
183	6	14:30:00	14:45:00
184	6	14:45:00	15:00:00
185	6	15:00:00	15:15:00
186	6	15:15:00	15:30:00
187	6	15:30:00	15:45:00
188	6	15:45:00	16:00:00
189	6	16:00:00	16:15:00
190	6	16:15:00	16:30:00
191	6	16:30:00	16:45:00
192	6	16:45:00	17:00:00
193	7	09:00:00	09:15:00
194	7	09:15:00	09:30:00
195	7	09:30:00	09:45:00
196	7	09:45:00	10:00:00
197	7	10:00:00	10:15:00
198	7	10:15:00	10:30:00
199	7	10:30:00	10:45:00
200	7	10:45:00	11:00:00
201	7	11:00:00	11:15:00
202	7	11:15:00	11:30:00
203	7	11:30:00	11:45:00
204	7	11:45:00	12:00:00
205	7	12:00:00	12:15:00
206	7	12:15:00	12:30:00
207	7	12:30:00	12:45:00
208	7	12:45:00	13:00:00
209	7	13:00:00	13:15:00
210	7	13:15:00	13:30:00
211	7	13:30:00	13:45:00
212	7	13:45:00	14:00:00
213	7	14:00:00	14:15:00
214	7	14:15:00	14:30:00
215	7	14:30:00	14:45:00
216	7	14:45:00	15:00:00
217	7	15:00:00	15:15:00
218	7	15:15:00	15:30:00
219	7	15:30:00	15:45:00
220	7	15:45:00	16:00:00
221	7	16:00:00	16:15:00
222	7	16:15:00	16:30:00
223	7	16:30:00	16:45:00
224	7	16:45:00	17:00:00
\.


--
-- TOC entry 3446 (class 0 OID 16412)
-- Dependencies: 218
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (employee_id, first_name, last_name) FROM stdin;
1	Jan	Kowalski
2	Robert	Nowak
3	Tomasz	Kowalski
4	Anna	Nowak
5	Piotr	Zieliński
6	Maria	Wiśniewska
7	Tomasz	Mazur
\.


--
-- TOC entry 3448 (class 0 OID 16418)
-- Dependencies: 220
-- Data for Name: reservations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.reservations (id, user_id, reservation_date, end_time, status, service_id, employee_id, start_time) FROM stdin;
\.


--
-- TOC entry 3450 (class 0 OID 16425)
-- Dependencies: 222
-- Data for Name: services; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.services (id, name, duration, price) FROM stdin;
1	strzyżenie męskie	00:30:00	45.00
2	Strzyżenie damskie	00:45:00	60.00
3	Koloryzacja włosów	01:30:00	120.00
4	Manicure hybrydowy	00:40:00	50.00
5	Pedicure klasyczny	01:00:00	80.00
6	Masaż relaksacyjny	01:00:00	100.00
\.


--
-- TOC entry 3452 (class 0 OID 16429)
-- Dependencies: 224
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (id, username, password, role, refresh_token) FROM stdin;
\.


--
-- TOC entry 3461 (class 0 OID 0)
-- Dependencies: 217
-- Name: available_hours_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.available_hours_id_seq', 224, true);


--
-- TOC entry 3462 (class 0 OID 0)
-- Dependencies: 219
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 7, true);


--
-- TOC entry 3463 (class 0 OID 0)
-- Dependencies: 221
-- Name: reservations_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.reservations_id_seq', 1, false);


--
-- TOC entry 3464 (class 0 OID 0)
-- Dependencies: 223
-- Name: services_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.services_id_seq', 6, true);


--
-- TOC entry 3287 (class 2606 OID 16440)
-- Name: available_hours available_hours_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.available_hours
    ADD CONSTRAINT available_hours_pkey PRIMARY KEY (id);


--
-- TOC entry 3289 (class 2606 OID 16442)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 3291 (class 2606 OID 16444)
-- Name: reservations reservations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations
    ADD CONSTRAINT reservations_pkey PRIMARY KEY (id);


--
-- TOC entry 3293 (class 2606 OID 16446)
-- Name: services services_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.services
    ADD CONSTRAINT services_pkey PRIMARY KEY (id);


--
-- TOC entry 3295 (class 2606 OID 16448)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 3297 (class 2606 OID 16486)
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- TOC entry 3299 (class 2606 OID 16451)
-- Name: reservations fk26cya250clfqgfl59s9vi9e8x; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations
    ADD CONSTRAINT fk26cya250clfqgfl59s9vi9e8x FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 3300 (class 2606 OID 16456)
-- Name: reservations fk3ww618ll729ubr3pb5krt9d3h; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations
    ADD CONSTRAINT fk3ww618ll729ubr3pb5krt9d3h FOREIGN KEY (service_id) REFERENCES public.services(id);


--
-- TOC entry 3298 (class 2606 OID 16461)
-- Name: available_hours fk_employee_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.available_hours
    ADD CONSTRAINT fk_employee_id FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


-- Completed on 2025-03-16 21:22:33

--
-- PostgreSQL database dump complete
--


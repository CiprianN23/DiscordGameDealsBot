--
-- PostgreSQL database dump
--

-- Dumped from database version 14.1
-- Dumped by pg_dump version 14.1

-- Started on 2021-12-02 14:29:28

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
-- TOC entry 3 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA public;


--
-- TOC entry 3340 (class 0 OID 0)
-- Dependencies: 3
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA public IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 211 (class 1259 OID 16754)
-- Name: discord_channels; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.discord_channels (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    channel_id numeric(20,1) NOT NULL,
    guild_id uuid NOT NULL
);


--
-- TOC entry 210 (class 1259 OID 16742)
-- Name: discord_guilds; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.discord_guilds (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    guild numeric(20,1) NOT NULL
);


--
-- TOC entry 212 (class 1259 OID 16765)
-- Name: discord_messages; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.discord_messages (
    message_id numeric(20,1) NOT NULL,
    reddit_post uuid NOT NULL,
    channel_id uuid NOT NULL,
    id uuid DEFAULT gen_random_uuid() NOT NULL
);


--
-- TOC entry 209 (class 1259 OID 16736)
-- Name: reddit_posts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.reddit_posts (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    perma_link character varying(120) NOT NULL,
    full_name character varying(20) NOT NULL
);

--
-- TOC entry 3186 (class 2606 OID 16759)
-- Name: discord_channels discord_channels_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.discord_channels
    ADD CONSTRAINT discord_channels_pkey PRIMARY KEY (id);


--
-- TOC entry 3182 (class 2606 OID 16747)
-- Name: discord_guilds discord_guild_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.discord_guilds
    ADD CONSTRAINT discord_guild_pkey PRIMARY KEY (id);


--
-- TOC entry 3184 (class 2606 OID 16782)
-- Name: discord_guilds discord_guilds_guild_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.discord_guilds
    ADD CONSTRAINT discord_guilds_guild_key UNIQUE (guild);


--
-- TOC entry 3188 (class 2606 OID 16770)
-- Name: discord_messages discord_messages_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.discord_messages
    ADD CONSTRAINT discord_messages_pkey PRIMARY KEY (id);


--
-- TOC entry 3180 (class 2606 OID 16741)
-- Name: reddit_posts reddit_post_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.reddit_posts
    ADD CONSTRAINT reddit_post_pkey PRIMARY KEY (id);


--
-- TOC entry 3189 (class 2606 OID 16760)
-- Name: discord_channels discordchannels_discordguilds_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.discord_channels
    ADD CONSTRAINT discordchannels_discordguilds_fkey FOREIGN KEY (guild_id) REFERENCES public.discord_guilds(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3190 (class 2606 OID 16771)
-- Name: discord_messages discordchannels_discordmessages_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.discord_messages
    ADD CONSTRAINT discordchannels_discordmessages_fkey FOREIGN KEY (channel_id) REFERENCES public.discord_channels(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3191 (class 2606 OID 16776)
-- Name: discord_messages redditposts_discordmessages_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.discord_messages
    ADD CONSTRAINT redditposts_discordmessages_fkey FOREIGN KEY (reddit_post) REFERENCES public.reddit_posts(id) ON UPDATE CASCADE ON DELETE CASCADE;


-- Completed on 2021-12-02 14:29:28

--
-- PostgreSQL database dump complete
--


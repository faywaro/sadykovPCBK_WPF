-- ============================================================
-- База данных: sadykovpcbk
-- Схема: app
-- Проект: sadykovPCBKpartner
-- Автор: Sadykov
-- Описание: Подсистема работы с партнёрами
--           АО "Пермская целлюлозно-бумажная компания" (ПЦБК)
-- ============================================================

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'app') THEN
        CREATE ROLE app WITH LOGIN PASSWORD '123456789';
    END IF;
END
$$;

-- CREATE DATABASE sadykovpcbk OWNER app;

CREATE SCHEMA IF NOT EXISTS app AUTHORIZATION app;
SET search_path TO app;

CREATE TABLE IF NOT EXISTS app.partner_types (
    id          SERIAL          PRIMARY KEY,
    type_name   VARCHAR(100)    NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS app.partners (
    id              SERIAL          PRIMARY KEY,
    type_id         INTEGER         NOT NULL REFERENCES app.partner_types(id) ON DELETE RESTRICT,
    company_name    VARCHAR(255)    NOT NULL,
    legal_address   VARCHAR(500)    NOT NULL,
    inn             VARCHAR(12)     NOT NULL UNIQUE,
    director_name   VARCHAR(255)    NOT NULL,
    phone           VARCHAR(20)     NOT NULL,
    email           VARCHAR(255)    NOT NULL,
    rating          INTEGER         NOT NULL DEFAULT 0 CHECK (rating >= 0),
    logo_path       VARCHAR(500)    NULL
);

CREATE TABLE IF NOT EXISTS app.products (
    id              SERIAL          PRIMARY KEY,
    article         VARCHAR(50)     NOT NULL UNIQUE,
    product_name    VARCHAR(255)    NOT NULL,
    product_type    VARCHAR(100)    NOT NULL,
    min_price       NUMERIC(12,2)   NOT NULL CHECK (min_price >= 0)
);

CREATE TABLE IF NOT EXISTS app.partner_sales (
    id              SERIAL          PRIMARY KEY,
    partner_id      INTEGER         NOT NULL REFERENCES app.partners(id) ON DELETE CASCADE,
    product_id      INTEGER         NOT NULL REFERENCES app.products(id) ON DELETE RESTRICT,
    quantity        INTEGER         NOT NULL CHECK (quantity > 0),
    sale_date       DATE            NOT NULL DEFAULT CURRENT_DATE
);

GRANT USAGE ON SCHEMA app TO app;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA app TO app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA app TO app;

-- Типы партнёров
INSERT INTO app.partner_types (type_name) VALUES
    ('ЗАО'), ('ООО'), ('ПАО'), ('ОАО'), ('АО')
ON CONFLICT (type_name) DO NOTHING;

-- Продукция ПЦБК
INSERT INTO app.products (article, product_name, product_type, min_price) VALUES
    ('PCBK-001', 'Гофрокартон трёхслойный Т-23',        'Гофрокартон',  18.50),
    ('PCBK-002', 'Гофрокартон пятислойный Т-24',        'Гофрокартон',  26.00),
    ('PCBK-003', 'Картон тарный марки КТ-1',            'Картон',       22.00),
    ('PCBK-004', 'Картон тарный марки КТ-2',            'Картон',       19.80),
    ('PCBK-005', 'Бумага для гофрирования БГ-100',      'Бумага',       14.30),
    ('PCBK-006', 'Бумага крафт мешочная 78 г/м',        'Бумага',       32.00),
    ('PCBK-007', 'Коробка гофрированная 400x300x200',   'Упаковка',     28.50),
    ('PCBK-008', 'Коробка гофрированная 600x400x300',   'Упаковка',     42.00),
    ('PCBK-009', 'Коробка архивная А4 330x245x310',     'Упаковка',     35.00),
    ('PCBK-010', 'Лоток гофрированный под продукты',    'Упаковка',     12.00),
    ('PCBK-011', 'Поддон бумажный 1200x800',            'Упаковка',     85.00),
    ('PCBK-012', 'Бумага офисная А4 80 г/м 500 л',     'Бумага',      280.00)
ON CONFLICT (article) DO NOTHING;

-- Партнёры ПЦБК
INSERT INTO app.partners
    (type_id, company_name, legal_address, inn, director_name, phone, email, rating)
VALUES
    (2, 'УралБумТорг',
     'г. Екатеринбург, ул. Малышева, 36', '6670123456',
     'Власов Сергей Николаевич', '+7 343 220 11 22', 'info@uralbumtorg.ru', 9),

    (2, 'ПромУпаковка',
     'г. Пермь, ул. Ленина, 58', '5902234567',
     'Кузнецов Алексей Владимирович', '+7 342 270 33 44', 'order@promupak.ru', 7),

    (5, 'КартонПлюс',
     'г. Казань, пр. Победы, 14', '1655345678',
     'Фаттахов Марат Рустамович', '+7 843 290 55 66', 'sales@kartonplus.ru', 10),

    (1, 'СибПакет',
     'г. Новосибирск, ул. Кирова, 29', '5407456789',
     'Морозов Дмитрий Александрович', '+7 383 310 77 88', 'sibpaket@mail.ru', 5),

    (2, 'ТД Гофра',
     'г. Самара, ул. Советской Армии, 181', '6315567890',
     'Степанов Павел Игоревич', '+7 846 330 99 00', 'gofra@td-gofra.ru', 8),

    (3, 'РосУпак',
     'г. Москва, Варшавское шоссе, 46', '7736678901',
     'Беляев Игорь Константинович', '+7 495 640 12 34', 'contact@rosupak.ru', 6),

    (2, 'ПакПром Урал',
     'г. Челябинск, ул. Труда, 87', '7451789012',
     'Григорьев Виктор Семёнович', '+7 351 260 56 78', 'pakprom@ural.ru', 4),

    (4, 'Экопак',
     'г. Нижний Новгород, ул. Горького, 117', '5262890123',
     'Романова Елена Сергеевна', '+7 831 430 34 56', 'info@ekopak-nn.ru', 7)
ON CONFLICT (inn) DO NOTHING;

-- История реализации
INSERT INTO app.partner_sales (partner_id, product_id, quantity, sale_date) VALUES
    (1, 1,  85000, '2023-03-15'),
    (1, 2,  40000, '2023-07-20'),
    (1, 7,  60000, '2023-11-05'),
    (1, 8,  30000, '2024-02-14'),
    (1, 3,  55000, '2024-05-10'),
    (2, 5,  20000, '2023-04-12'),
    (2, 6,  15000, '2023-09-18'),
    (2, 9,  10000, '2024-01-22'),
    (3, 1, 120000, '2023-01-10'),
    (3, 2,  80000, '2023-06-25'),
    (3, 3,  70000, '2023-10-30'),
    (3, 7,  90000, '2024-03-18'),
    (3, 8,  50000, '2024-07-04'),
    (4, 5,   8000, '2023-05-20'),
    (4, 12,  2000, '2024-02-28'),
    (5, 1,  35000, '2023-02-14'),
    (5, 7,  25000, '2023-08-09'),
    (5, 11,  5000, '2024-04-17'),
    (6, 6,  18000, '2023-06-01'),
    (6, 8,  12000, '2023-12-15'),
    (6, 9,   9000, '2024-06-20'),
    (7, 10,  4000, '2023-07-11'),
    (7, 12,  1500, '2024-01-05'),
    (8, 5,  22000, '2023-09-03'),
    (8, 4,  16000, '2024-03-27'),
    (8, 10,  8000, '2024-08-14');

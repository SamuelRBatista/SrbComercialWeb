CREATE TABLE IF NOT EXISTS states (
    id SERIAL PRIMARY KEY,
    name VARCHAR(120) NOT NULL,
    uf VARCHAR(2) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS cities (
    id SERIAL PRIMARY KEY,
    name VARCHAR(160) NOT NULL,
    state_id INTEGER NOT NULL REFERENCES states(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(160) NOT NULL
);

CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(120) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    email VARCHAR(255) NOT NULL,
    role VARCHAR(80) NOT NULL
);

CREATE TABLE IF NOT EXISTS clients (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    cpf VARCHAR(30) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone_number VARCHAR(40) NOT NULL,
    address VARCHAR(255) NOT NULL,
    neighborhood VARCHAR(160) NOT NULL,
    zip_code VARCHAR(20) NOT NULL,
    state_id INTEGER NOT NULL REFERENCES states(id),
    city_id INTEGER NOT NULL REFERENCES cities(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS suppliers (
    id SERIAL PRIMARY KEY,
    cnpj VARCHAR(30) NOT NULL,
    name VARCHAR(200) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone_number VARCHAR(40) NOT NULL,
    address VARCHAR(255) NOT NULL,
    neighborhood VARCHAR(160) NOT NULL,
    zip_code VARCHAR(20) NOT NULL,
    state_id INTEGER NOT NULL REFERENCES states(id),
    city_id INTEGER NOT NULL REFERENCES cities(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    short_description TEXT NOT NULL DEFAULT '',
    sku VARCHAR(120) NOT NULL UNIQUE,
    bar_code VARCHAR(120) NOT NULL DEFAULT '',
    main_image_url TEXT NOT NULL DEFAULT '',
    category_id INTEGER NOT NULL REFERENCES categories(id),
    supplier_id INTEGER REFERENCES suppliers(id) ON DELETE SET NULL,
    price NUMERIC(12,2) NOT NULL DEFAULT 0,
    cost_price NUMERIC(12,2) NOT NULL DEFAULT 0,
    wholesale_price NUMERIC(12,2),
    wholesale_min_quantity INTEGER,
    stock_quantity INTEGER NOT NULL DEFAULT 0,
    minimum_stock INTEGER NOT NULL DEFAULT 0,
    maximum_stock INTEGER,
    unit_of_measure VARCHAR(20) NOT NULL DEFAULT 'UN',
    weight NUMERIC(12,3),
    height NUMERIC(12,3),
    width NUMERIC(12,3),
    depth NUMERIC(12,3),
    brand VARCHAR(120),
    model VARCHAR(120),
    color VARCHAR(80),
    size VARCHAR(80),
    material VARCHAR(120),
    manufacturer VARCHAR(160),
    manufacture_date TIMESTAMP,
    expiration_date TIMESTAMP,
    meta_title VARCHAR(255),
    meta_description TEXT,
    meta_keywords TEXT,
    slug VARCHAR(255),
    tags TEXT,
    is_active BOOLEAN NOT NULL DEFAULT false,
    is_featured BOOLEAN NOT NULL DEFAULT false,
    is_new BOOLEAN NOT NULL DEFAULT true,
    is_digital BOOLEAN NOT NULL DEFAULT false,
    has_variants BOOLEAN NOT NULL DEFAULT false,
    average_rating NUMERIC(4,2) NOT NULL DEFAULT 0,
    total_reviews INTEGER NOT NULL DEFAULT 0,
    total_sales INTEGER NOT NULL DEFAULT 0,
    views_count INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by_user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    updated_by_user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    observations TEXT NOT NULL DEFAULT '',
    internal_notes TEXT NOT NULL DEFAULT '',
    promotional_price NUMERIC(12,2),
    promotion_start_date TIMESTAMP,
    promotion_end_date TIMESTAMP,
    promotion_id INTEGER,
    status INTEGER NOT NULL DEFAULT 0,
    visibility INTEGER NOT NULL DEFAULT 0,
    is_track_inventory BOOLEAN NOT NULL DEFAULT true,
    allow_backorder BOOLEAN NOT NULL DEFAULT false,
    expected_stock_date TIMESTAMP,
    video_url TEXT,
    warranty_info TEXT,
    shipping_info TEXT,
    returns_policy TEXT,
    requires_shipping BOOLEAN NOT NULL DEFAULT true
);

CREATE TABLE IF NOT EXISTS product_images (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    url TEXT NOT NULL,
    is_main BOOLEAN NOT NULL DEFAULT false,
    description TEXT,
    "order" INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS product_attributes (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    "key" VARCHAR(120) NOT NULL,
    "value" TEXT NOT NULL,
    "group" VARCHAR(120),
    display_order INTEGER NOT NULL DEFAULT 0,
    is_visible BOOLEAN NOT NULL DEFAULT true,
    is_filterable BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS product_variants (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    name VARCHAR(160) NOT NULL,
    sku VARCHAR(120) NOT NULL,
    bar_code VARCHAR(120),
    price NUMERIC(12,2) NOT NULL DEFAULT 0,
    cost_price NUMERIC(12,2),
    stock_quantity INTEGER NOT NULL DEFAULT 0,
    image_url TEXT,
    is_active BOOLEAN NOT NULL DEFAULT true,
    minimum_stock INTEGER,
    promotional_price NUMERIC(12,2),
    promotion_start_date TIMESTAMP,
    promotion_end_date TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS variant_attributes (
    id SERIAL PRIMARY KEY,
    variant_id INTEGER NOT NULL REFERENCES product_variants(id) ON DELETE CASCADE,
    "key" VARCHAR(120) NOT NULL,
    "value" TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS stock_movements (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    variant_id INTEGER REFERENCES product_variants(id) ON DELETE SET NULL,
    quantity INTEGER NOT NULL,
    type VARCHAR(80) NOT NULL,
    reason VARCHAR(255),
    document_number VARCHAR(120),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    user_name VARCHAR(160),
    unit_cost NUMERIC(12,2),
    total_cost NUMERIC(12,2),
    previous_stock INTEGER,
    new_stock INTEGER,
    notes TEXT
);

CREATE TABLE IF NOT EXISTS product_reviews (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    user_name VARCHAR(160),
    user_email VARCHAR(255),
    rating INTEGER NOT NULL DEFAULT 0,
    title VARCHAR(255),
    comment TEXT,
    is_approved BOOLEAN NOT NULL DEFAULT false,
    is_verified_purchase BOOLEAN NOT NULL DEFAULT false,
    is_recommended BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    approved_at TIMESTAMP,
    helpful_count INTEGER NOT NULL DEFAULT 0,
    not_helpful_count INTEGER NOT NULL DEFAULT 0,
    admin_response TEXT,
    admin_response_at TIMESTAMP,
    images TEXT
);

INSERT INTO states (name, uf) VALUES
    ('Sao Paulo', 'SP'),
    ('Rio de Janeiro', 'RJ'),
    ('Minas Gerais', 'MG')
ON CONFLICT (uf) DO NOTHING;

INSERT INTO cities (name, state_id)
SELECT 'Sao Paulo', id FROM states WHERE uf = 'SP'
AND NOT EXISTS (SELECT 1 FROM cities WHERE name = 'Sao Paulo' AND state_id = states.id);

INSERT INTO cities (name, state_id)
SELECT 'Rio de Janeiro', id FROM states WHERE uf = 'RJ'
AND NOT EXISTS (SELECT 1 FROM cities WHERE name = 'Rio de Janeiro' AND state_id = states.id);

INSERT INTO cities (name, state_id)
SELECT 'Belo Horizonte', id FROM states WHERE uf = 'MG'
AND NOT EXISTS (SELECT 1 FROM cities WHERE name = 'Belo Horizonte' AND state_id = states.id);

INSERT INTO categories (name)
SELECT seed.name
FROM (VALUES
    ('Temperos e Especiarias'),
    ('Pimentas'),
    ('Ervas Desidratadas'),
    ('Sais Especiais'),
    ('Molhos Artesanais'),
    ('Kits para Churrasco'),
    ('Azeites e Vinagres'),
    ('Graos e Sementes'),
    ('Presentes Gourmet'),
    ('Utensilios de Cozinha')
) AS seed(name)
WHERE NOT EXISTS (
    SELECT 1 FROM categories current_category WHERE current_category.name = seed.name
);

INSERT INTO suppliers
    (cnpj, name, email, phone_number, address, neighborhood, zip_code, state_id, city_id)
SELECT seed.cnpj, seed.name, seed.email, seed.phone_number, seed.address,
       seed.neighborhood, seed.zip_code, states.id, cities.id
FROM (VALUES
    ('12.345.678/0001-01', 'Casa das Especiarias Ltda', 'contato@casadasespeciarias.example', '(11) 3000-1001', 'Rua dos Aromas, 120', 'Vila Mariana', '04110-000', 'SP', 'Sao Paulo'),
    ('23.456.789/0001-02', 'Sabor da Serra Alimentos', 'vendas@sabordaserra.example', '(31) 3000-1002', 'Avenida Central, 450', 'Funcionarios', '30130-000', 'MG', 'Belo Horizonte'),
    ('34.567.890/0001-03', 'Pimenta Real Comercio', 'comercial@pimentareal.example', '(21) 3000-1003', 'Rua do Mercado, 88', 'Centro', '20010-000', 'RJ', 'Rio de Janeiro'),
    ('45.678.901/0001-04', 'Vale Verde Naturais', 'atendimento@valeverde.example', '(11) 3000-1004', 'Rua das Palmeiras, 72', 'Moema', '04520-000', 'SP', 'Sao Paulo'),
    ('56.789.012/0001-05', 'Monte Bom Ingredientes', 'vendas@montebom.example', '(31) 3000-1005', 'Rua das Montanhas, 210', 'Savassi', '30140-000', 'MG', 'Belo Horizonte'),
    ('67.890.123/0001-06', 'Gourmet Brasil Distribuidora', 'contato@gourmetbrasil.example', '(21) 3000-1006', 'Avenida Atlantica, 900', 'Copacabana', '22010-000', 'RJ', 'Rio de Janeiro'),
    ('78.901.234/0001-07', 'Raizes do Campo Produtos', 'comercial@raizesdocampo.example', '(11) 3000-1007', 'Estrada do Campo, 35', 'Butanta', '05508-000', 'SP', 'Sao Paulo'),
    ('89.012.345/0001-08', 'Aroma & Cia Ingredientes', 'contato@aromaecia.example', '(31) 3000-1008', 'Rua do Comercio, 156', 'Lourdes', '30170-000', 'MG', 'Belo Horizonte'),
    ('90.123.456/0001-09', 'Selecao do Chef', 'vendas@selecaodochef.example', '(21) 3000-1009', 'Rua Gourmet, 64', 'Botafogo', '22290-000', 'RJ', 'Rio de Janeiro'),
    ('01.234.567/0001-10', 'Mercado dos Sabores', 'atendimento@mercadodossabores.example', '(11) 3000-1010', 'Rua do Sabor, 300', 'Pinheiros', '05422-000', 'SP', 'Sao Paulo')
) AS seed(cnpj, name, email, phone_number, address, neighborhood, zip_code, uf, city_name)
JOIN states ON states.uf = seed.uf
JOIN cities ON cities.name = seed.city_name AND cities.state_id = states.id
WHERE NOT EXISTS (
    SELECT 1 FROM suppliers current_supplier WHERE current_supplier.cnpj = seed.cnpj
);

INSERT INTO clients
    (name, cpf, email, phone_number, address, neighborhood, zip_code, state_id, city_id)
SELECT seed.name, seed.cpf, seed.email, seed.phone_number, seed.address,
       seed.neighborhood, seed.zip_code, states.id, cities.id
FROM (VALUES
    ('Ana Beatriz Martins', '111.222.333-01', 'ana.martins@example.com', '(11) 98888-1001', 'Rua das Flores, 15', 'Pinheiros', '05422-100', 'SP', 'Sao Paulo'),
    ('Bruno Henrique Costa', '111.222.333-02', 'bruno.costa@example.com', '(31) 98888-1002', 'Rua Ouro Preto, 82', 'Barro Preto', '30180-120', 'MG', 'Belo Horizonte'),
    ('Carolina Mendes Rocha', '111.222.333-03', 'carolina.rocha@example.com', '(21) 98888-1003', 'Rua Voluntarios, 210', 'Botafogo', '22270-000', 'RJ', 'Rio de Janeiro'),
    ('Daniel Augusto Lima', '111.222.333-04', 'daniel.lima@example.com', '(11) 98888-1004', 'Avenida Jabaquara, 440', 'Saude', '04046-000', 'SP', 'Sao Paulo'),
    ('Elisa Fernanda Alves', '111.222.333-05', 'elisa.alves@example.com', '(31) 98888-1005', 'Rua da Bahia, 1200', 'Lourdes', '30160-011', 'MG', 'Belo Horizonte'),
    ('Felipe Moreira Santos', '111.222.333-06', 'felipe.santos@example.com', '(21) 98888-1006', 'Rua Barata Ribeiro, 90', 'Copacabana', '22011-001', 'RJ', 'Rio de Janeiro'),
    ('Gabriela Souza Nunes', '111.222.333-07', 'gabriela.nunes@example.com', '(11) 98888-1007', 'Rua Harmonia, 55', 'Vila Madalena', '05435-001', 'SP', 'Sao Paulo'),
    ('Henrique Oliveira Dias', '111.222.333-08', 'henrique.dias@example.com', '(31) 98888-1008', 'Rua dos Timbiras, 700', 'Funcionarios', '30140-060', 'MG', 'Belo Horizonte'),
    ('Isabela Castro Freire', '111.222.333-09', 'isabela.freire@example.com', '(21) 98888-1009', 'Rua Jardim Botanico, 340', 'Jardim Botanico', '22460-000', 'RJ', 'Rio de Janeiro'),
    ('Joao Victor Ribeiro', '111.222.333-10', 'joao.ribeiro@example.com', '(11) 98888-1010', 'Rua Cardeal Arcoverde, 180', 'Pinheiros', '05408-000', 'SP', 'Sao Paulo')
) AS seed(name, cpf, email, phone_number, address, neighborhood, zip_code, uf, city_name)
JOIN states ON states.uf = seed.uf
JOIN cities ON cities.name = seed.city_name AND cities.state_id = states.id
WHERE NOT EXISTS (
    SELECT 1 FROM clients current_client WHERE current_client.cpf = seed.cpf
);

INSERT INTO products
    (name, description, short_description, sku, bar_code, category_id, supplier_id,
     price, cost_price, stock_quantity, minimum_stock, unit_of_measure, weight,
     brand, is_active, is_featured, is_new, status, visibility, slug, tags)
SELECT seed.name, seed.description, seed.short_description, seed.sku, seed.bar_code,
       categories.id, suppliers.id, seed.price, seed.cost_price, seed.stock_quantity,
       seed.minimum_stock, 'UN', seed.weight, seed.brand, true, seed.is_featured,
       true, 1, 3, seed.slug, seed.tags
FROM (VALUES
    ('Pimenta do Reino Premium 100g', 'Pimenta do reino moida na hora, com aroma intenso e acabamento fresco.', 'Pimenta do reino premium para uso diario.', 'TEM-PIM-001', '7890000000011', 'Pimentas', 'Aroma & Cia Ingredientes', 18.90::numeric, 10.50::numeric, 84, 10, 0.100::numeric, 'Aroma & Cia', true, 'pimenta-do-reino-premium-100g', 'pimenta,temperos'),
    ('Mix de Ervas Finas 80g', 'Selecao equilibrada de ervas desidratadas para carnes, massas e saladas.', 'Mix de ervas finas para receitas especiais.', 'TEM-ERV-002', '7890000000012', 'Ervas Desidratadas', 'Vale Verde Naturais', 16.50::numeric, 8.90::numeric, 65, 8, 0.080::numeric, 'Vale Verde', false, 'mix-de-ervas-finas-80g', 'ervas,temperos'),
    ('Sal Rosa do Himalaia 500g', 'Sal mineral de granulometria uniforme para finalizacao e preparo.', 'Sal rosa para cozinha e mesa.', 'TEM-SAL-003', '7890000000013', 'Sais Especiais', 'Sabor da Serra Alimentos', 24.90::numeric, 14.00::numeric, 42, 6, 0.500::numeric, 'Sabor da Serra', true, 'sal-rosa-do-himalaia-500g', 'sal,gourmet'),
    ('Molho de Pimenta Defumada 150ml', 'Molho artesanal com defumacao natural e ardencia equilibrada.', 'Molho artesanal defumado.', 'TEM-MOL-004', '7890000000014', 'Molhos Artesanais', 'Pimenta Real Comercio', 21.90::numeric, 12.50::numeric, 37, 5, 0.150::numeric, 'Pimenta Real', true, 'molho-de-pimenta-defumada-150ml', 'molhos,pimenta'),
    ('Kit Churrasco do Chef', 'Selecao com sal de parrilla, chimichurri e pimenta para churrasco.', 'Kit completo para churrasco.', 'TEM-KIT-005', '7890000000015', 'Kits para Churrasco', 'Selecao do Chef', 69.90::numeric, 39.90::numeric, 28, 4, 0.750::numeric, 'Selecao do Chef', true, 'kit-churrasco-do-chef', 'kit,churrasco'),
    ('Azeite Extra Virgem 500ml', 'Azeite extra virgem de sabor frutado e baixa acidez.', 'Azeite extra virgem para finalizar.', 'TEM-AZE-006', '7890000000016', 'Azeites e Vinagres', 'Mercado dos Sabores', 44.90::numeric, 27.00::numeric, 31, 5, 0.500::numeric, 'Mercado dos Sabores', false, 'azeite-extra-virgem-500ml', 'azeite,gourmet'),
    ('Chia Premium 250g', 'Sementes selecionadas para complementar receitas, iogurtes e saladas.', 'Chia premium para sua rotina.', 'TEM-GRA-007', '7890000000017', 'Graos e Sementes', 'Raizes do Campo Produtos', 19.90::numeric, 11.20::numeric, 53, 7, 0.250::numeric, 'Raizes do Campo', false, 'chia-premium-250g', 'graos,sementes'),
    ('Kit Presente Sabores do Brasil', 'Composicao presenteavel com temperos, molhos e sal especial.', 'Kit gourmet para presentear.', 'TEM-PRE-008', '7890000000018', 'Presentes Gourmet', 'Gourmet Brasil Distribuidora', 119.90::numeric, 72.00::numeric, 16, 3, 1.200::numeric, 'Gourmet Brasil', true, 'kit-presente-sabores-do-brasil', 'presente,gourmet'),
    ('Canela em Po 100g', 'Canela aromatica moida, ideal para bebidas, doces e preparos especiais.', 'Canela em po de aroma marcante.', 'TEM-ERV-009', '7890000000019', 'Ervas Desidratadas', 'Casa das Especiarias Ltda', 12.90::numeric, 6.50::numeric, 76, 10, 0.100::numeric, 'Casa das Especiarias', false, 'canela-em-po-100g', 'canela,temperos'),
    ('Pimenta Rosa 50g', 'Pimenta rosa selecionada para finalizacao de pratos e molhos.', 'Pimenta rosa para finalizacao.', 'TEM-PIM-010', '7890000000020', 'Pimentas', 'Monte Bom Ingredientes', 22.90::numeric, 13.50::numeric, 39, 5, 0.050::numeric, 'Monte Bom', true, 'pimenta-rosa-50g', 'pimenta,gourmet')
) AS seed(name, description, short_description, sku, bar_code, category_name, supplier_name, price, cost_price, stock_quantity, minimum_stock, weight, brand, is_featured, slug, tags)
JOIN categories ON categories.name = seed.category_name
JOIN suppliers ON suppliers.name = seed.supplier_name
WHERE NOT EXISTS (
    SELECT 1 FROM products current_product WHERE current_product.sku = seed.sku
);

UPDATE products
SET main_image_url = CASE sku
    WHEN 'TEM-PIM-001' THEN 'https://placehold.co/600x600/8b1e3f/ffffff?text=Pimenta+do+Reino'
    WHEN 'TEM-ERV-002' THEN 'https://placehold.co/600x600/4f772d/ffffff?text=Ervas+Finas'
    WHEN 'TEM-SAL-003' THEN 'https://placehold.co/600x600/d4a017/ffffff?text=Sal+Rosa'
    WHEN 'TEM-MOL-004' THEN 'https://placehold.co/600x600/9b2226/ffffff?text=Molho+Defumado'
    WHEN 'TEM-KIT-005' THEN 'https://placehold.co/600x600/6b4226/ffffff?text=Kit+Churrasco'
    WHEN 'TEM-AZE-006' THEN 'https://placehold.co/600x600/7f8c1a/ffffff?text=Azeite'
    WHEN 'TEM-GRA-007' THEN 'https://placehold.co/600x600/8d6e63/ffffff?text=Chia'
    WHEN 'TEM-PRE-008' THEN 'https://placehold.co/600x600/264653/ffffff?text=Kit+Presente'
    WHEN 'TEM-ERV-009' THEN 'https://placehold.co/600x600/b5651d/ffffff?text=Canela'
    WHEN 'TEM-PIM-010' THEN 'https://placehold.co/600x600/c1121f/ffffff?text=Pimenta+Rosa'
    ELSE main_image_url
END
WHERE sku IN (
    'TEM-PIM-001', 'TEM-ERV-002', 'TEM-SAL-003', 'TEM-MOL-004', 'TEM-KIT-005',
    'TEM-AZE-006', 'TEM-GRA-007', 'TEM-PRE-008', 'TEM-ERV-009', 'TEM-PIM-010'
);
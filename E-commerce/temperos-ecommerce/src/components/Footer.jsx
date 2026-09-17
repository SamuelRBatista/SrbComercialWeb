import React from 'react';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="footer">
      <div className="footer-content">
        {/* Seção 1: Sobre */}
        <div className="footer-section">
          <h3>🌿 Temperos Finos</h3>
          <p>
            Oferecemos os melhores temperos e especiarias selecionados 
            para realçar o sabor dos seus pratos. Qualidade e sabor em 
            cada grão.
          </p>
          <div className="social-links">
            <a href="#" aria-label="Instagram">📷</a>
            <a href="#" aria-label="Facebook">👍</a>
            <a href="#" aria-label="WhatsApp">💬</a>
            <a href="#" aria-label="YouTube">▶️</a>
          </div>
        </div>

        {/* Seção 2: Links rápidos */}
        <div className="footer-section">
          <h4>Links Rápidos</h4>
          <ul>
            <li><a href="#">Início</a></li>
            <li><a href="#">Produtos</a></li>
            <li><a href="#">Categorias</a></li>
            <li><a href="#">Ofertas</a></li>
            <li><a href="#">Blog</a></li>
          </ul>
        </div>

        {/* Seção 3: Categorias */}
        <div className="footer-section">
          <h4>Categorias</h4>
          <ul>
            <li><a href="#">Pimentas</a></li>
            <li><a href="#">Temperos em Pó</a></li>
            <li><a href="#">Ervas</a></li>
            <li><a href="#">Grãos</a></li>
            <li><a href="#">Kits Especiais</a></li>
          </ul>
        </div>

        {/* Seção 4: Contato */}
        <div className="footer-section">
          <h4>Contato</h4>
          <ul>
            <li>📞 (11) 99999-9999</li>
            <li>📧 contato@temperosfinos.com</li>
            <li>📍 Rua dos Temperos, 123</li>
            <li>🕐 Seg-Sex: 8h - 18h</li>
          </ul>
        </div>
      </div>

      {/* Seção de newsletter */}
      <div className="footer-newsletter">
        <div className="newsletter-content">
          <h4>📧 Receba nossas ofertas</h4>
          <p>Cadastre-se e receba promoções exclusivas!</p>
          <form onSubmit={(e) => e.preventDefault()} className="newsletter-form">
            <input 
              type="email" 
              placeholder="Seu melhor e-mail"
              required
            />
            <button type="submit">Cadastrar</button>
          </form>
        </div>
      </div>

      {/* Rodapé inferior */}
      <div className="footer-bottom">
        <div className="footer-bottom-content">
          <p>
            © {currentYear} Temperos Finos. Todos os direitos reservados.
          </p>
          <div className="footer-payments">
            <span>💳</span>
            <span>🏦</span>
            <span>📱</span>
            <span>💲</span>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
import React, { useState } from 'react';
import { CartProvider } from './contexts/CartContext';
import Header from './components/Header';
import ProductList from './components/ProductList';
import ShoppingCart from './components/ShoppingCart';
import Footer from './components/Footer';
import './App.css';

function App() {
  const [isCartOpen, setIsCartOpen] = useState(false);

  return (
    <CartProvider>
      <div className="app">
        <Header onCartClick={() => setIsCartOpen(true)} />
        <main className="main-content">
          <div className="container">
            <ProductList />
          </div>
        </main>
        <Footer />
        {isCartOpen && <ShoppingCart onClose={() => setIsCartOpen(false)} />}
      </div>
    </CartProvider>
  );
}

export default App;
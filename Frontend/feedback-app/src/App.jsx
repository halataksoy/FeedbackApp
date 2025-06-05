import { useState } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [form, setForm] = useState({ name: '', email: '', message: '' });
  const [success, setSuccess] = useState(false);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await axios.post('https://localhost:7062/api/Feedback', {
        name: form.name,
        email: form.email,
        message: form.message
      });
      setSuccess(true);
      setForm({ name: '', email: '', message: '' });
    } catch (error) {
      if (error.response) {
        console.error('Sunucu hatası:', error.response.data);
        alert(`Hata: ${error.response.data?.message || 'Bilinmeyen sunucu hatası'}`);
      } else if (error.request) {
        console.error('Sunucudan cevap alınamadı:', error.request);
        alert('Sunucudan cevap alınamadı.');
      } else {
        console.error('İstek hatası:', error.message);
        alert('İstek oluşturulurken hata oluştu.');
      }
    }
  };

  return (
    <div className="form-wrapper">
      <div className="form-container">
      <h1 className="form-title">Feedback App</h1>
        <form onSubmit={handleSubmit} className="feedback-form">
          <input
            type="text"
            name="name"
            placeholder="Adınız"
            value={form.name}
            onChange={handleChange}
            required
          />
          <input
            type="email"
            name="email"
            placeholder="E-posta"
            value={form.email}
            onChange={handleChange}
            required
          />
          <textarea
            name="message"
            placeholder="Mesajınız"
            value={form.message}
            onChange={handleChange}
            required
          />
          <button type="submit">Gönder</button>
          {success && <p className="success-message">Geri bildiriminiz alındı!</p>}
        </form>
      </div>
    </div>
  );
  
}

export default App;


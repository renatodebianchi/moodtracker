import React, { useState } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField, Stack, Alert } from '@mui/material';

interface RegisterProps {
  open: boolean;
  onClose: () => void;
  backendUrl: string;
  onRegisterLogin?: (token: string) => void;
}

const Register: React.FC<RegisterProps> = ({ open, onClose, backendUrl, onRegisterLogin }) => {
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(false);
    setLoading(true);
    try {
      const resp = await fetch(`${backendUrl}/api/users`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ nome, email, senha })
      });
      if (!resp.ok) {
        const err = await resp.text();
        setError('Erro ao cadastrar: ' + err);
        setLoading(false);
        return;
      }
      setSuccess(true);
      setNome(''); setEmail(''); setSenha('');
      // Login automático após cadastro
      if (onRegisterLogin) {
        // Tenta login
        const loginResp = await fetch(`${backendUrl}/api/users/login`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email, senha })
        });
        if (loginResp.ok) {
          const loginData = await loginResp.json();
          if (loginData.token) onRegisterLogin(loginData.token);
        }
      }
    } catch (err) {
      setError('Erro ao conectar ao servidor.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>Criar nova conta</DialogTitle>
      <DialogContent>
        <form onSubmit={handleSubmit}>
          <Stack spacing={2} mt={1}>
            <TextField label="Nome" value={nome} onChange={e => setNome(e.target.value)} fullWidth required />
            <TextField label="E-mail" type="email" value={email} onChange={e => setEmail(e.target.value)} fullWidth required />
            <TextField label="Senha" type="password" value={senha} onChange={e => setSenha(e.target.value)} fullWidth required />
            {error && <Alert severity="error">{error}</Alert>}
            {success && <Alert severity="success">Conta criada com sucesso! Faça login.</Alert>}
          </Stack>
        </form>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="secondary">Cancelar</Button>
        <Button onClick={handleSubmit} variant="contained" color="primary" disabled={loading}>Cadastrar</Button>
      </DialogActions>
    </Dialog>
  );
};

export default Register;

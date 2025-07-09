import React, { useState } from 'react';
import { Box, Paper, Typography, TextField, Button, Alert, Stack } from '@mui/material';
//import { GoogleLogin } from '@react-oauth/google';
import Register from './Register';

interface LoginProps {
  onLogin: (token: string) => void;
  backendUrl: string;
}

const Login: React.FC<LoginProps> = ({ onLogin, backendUrl }) => {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [registerOpen, setRegisterOpen] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const resp = await fetch(`${backendUrl}/api/users/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, senha })
      });
      if (!resp.ok) {
        setError('Usuário ou senha inválidos.');
        setLoading(false);
        return;
      }
      const data = await resp.json();
      if (data.token) {
        onLogin(data.token);
      } else {
        setError('Resposta inesperada do servidor.');
      }
    } catch (err) {
      setError('Erro ao conectar ao servidor.');
    } finally {
      setLoading(false);
    }
  };

  // SSO Google callback
  /*const handleGoogleSuccess = async (credentialResponse: any) => {
    if (!credentialResponse.credential) return;
    try {
      const resp = await fetch(`${backendUrl}/api/users/google`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ token: credentialResponse.credential })
      });
      const data = await resp.json();
      if (data.token) onLogin(data.token);
      else setError('Falha no login com Google.');
    } catch {
      setError('Erro ao conectar com Google.');
    }
  };*/

  return (
    <Box minHeight="100vh" width="100vw" display="flex" alignItems="center" justifyContent="center" sx={{
      background: 'linear-gradient(135deg, #a5b4fc 0%, #f8fafc 100%)',
      position: 'relative',
      overflow: 'hidden',
    }}>
      {/* Emojis de fundo */}
      <Box sx={{
        position: 'absolute',
        top: 0,
        left: 0,
        width: '100%',
        height: '100%',
        pointerEvents: 'none',
        zIndex: 0,
      }}>
        <Box sx={{ position: 'absolute', top: 40, left: 30, fontSize: 60, opacity: 0.18 }}>😁</Box>
        <Box sx={{ position: 'absolute', top: 120, right: 60, fontSize: 80, opacity: 0.13 }}>😐</Box>
        <Box sx={{ position: 'absolute', bottom: 80, left: 80, fontSize: 70, opacity: 0.15 }}>🤩</Box>
        <Box sx={{ position: 'absolute', bottom: 30, right: 40, fontSize: 55, opacity: 0.16 }}>🥵</Box>
        <Box sx={{ position: 'absolute', top: '50%', left: '50%', fontSize: 100, opacity: 0.08, transform: 'translate(-50%, -50%)' }}>😪</Box>
      </Box>
      <Paper elevation={8} sx={{
        p: { xs: 2, sm: 4 },
        borderRadius: 5,
        minWidth: { xs: '90vw', sm: 350 },
        maxWidth: 400,
        width: '100%',
        mx: 'auto',
        background: 'rgba(255,255,255,0.97)',
        zIndex: 1,
        boxShadow: '0 8px 32px 0 rgba(31, 38, 135, 0.18)',
        border: '1.5px solid #a5b4fc',
      }}>
        <Stack spacing={2} alignItems="center">
          <Typography variant="h4" fontWeight={800} color="#6366f1" mb={1} textAlign="center" letterSpacing={1}>
            Mood Tracking
          </Typography>
          <Typography variant="h6" fontWeight={600} color="#3730a3" mb={1} textAlign="center">
            Bem-vindo(a) de volta!
          </Typography>
          <Box component="form" width="100%" onSubmit={handleSubmit}>
            <TextField
              label="E-mail"
              type="email"
              value={email}
              onChange={e => setEmail(e.target.value)}
              fullWidth
              required
              sx={{ mb: 2 }}
            />
            <TextField
              label="Senha"
              type="password"
              value={senha}
              onChange={e => setSenha(e.target.value)}
              fullWidth
              required
              sx={{ mb: 2 }}
            />
            <Button type="submit" variant="contained" color="primary" fullWidth size="large" disabled={loading} sx={{
              fontWeight: 700,
              borderRadius: 2,
              py: 1.2,
              fontSize: 17,
              background: 'linear-gradient(90deg, #6366f1 0%, #818cf8 100%)',
              boxShadow: '0 2px 8px #a5b4fc55',
              letterSpacing: 1,
              '&:hover': {
                background: 'linear-gradient(90deg, #818cf8 0%, #6366f1 100%)',
              },
            }}>
              {loading ? 'Entrando...' : 'Entrar'}
            </Button>
          </Box>
          <Button color="secondary" onClick={() => setRegisterOpen(true)} sx={{ mt: 1, fontWeight: 600, letterSpacing: 0.5 }}>
            Criar nova conta
          </Button>
          {/* <GoogleLogin
            onSuccess={handleGoogleSuccess}
            onError={() => setError('Erro no login com Google.')}
            width="100%"
            useOneTap
          /> */}
          {/* Apple SSO pode ser adicionado aqui com um botão customizado */}
          {error && <Alert severity="error" sx={{ width: '100%' }}>{error}</Alert>}
        </Stack>
        <Register open={registerOpen} onClose={() => setRegisterOpen(false)} backendUrl={backendUrl} onRegisterLogin={onLogin} />
      </Paper>
    </Box>
  );
};

export default Login;

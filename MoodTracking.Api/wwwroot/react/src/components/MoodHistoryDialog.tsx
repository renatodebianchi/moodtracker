import React, { useEffect, useState } from 'react';
import { Dialog, DialogTitle, DialogContent, IconButton, Box, CircularProgress, TextField, MenuItem, Button } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { Line } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  Tooltip,
  Legend
} from 'chart.js';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

ChartJS.register(LineElement, PointElement, LinearScale, CategoryScale, Tooltip, Legend);

interface MoodHistoryDialogProps {
  open: boolean;
  onClose: () => void;
  backendUrl: string;
  token: string;
  userName?: string;
}

const MoodHistoryDialog: React.FC<MoodHistoryDialogProps> = ({ open, onClose, backendUrl, token, userName }) => {
  const [loading, setLoading] = useState(false);
  const [data, setData] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [days, setDays] = useState(30);
  const chartRef = React.useRef<any>(null);

  useEffect(() => {
    if (!open) return;
    setLoading(true);
    setError(null);
    fetch(`${backendUrl}/api/moodentries?days=${days}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    })
      .then(async resp => {
        if (!resp.ok) throw new Error(await resp.text());
        return resp.json();
      })
      .then(setData)
      .catch(err => setError('Erro ao buscar histórico: ' + err.message))
      .finally(() => setLoading(false));
  }, [open, backendUrl, token, days]);

  // Filtra os dados conforme o período selecionado
  const filteredData = React.useMemo(() => {
    if (days === 0) {
      // Apenas registros de hoje
      const today = new Date();
      return data.filter((entry: any) => {
        const d = new Date(entry.dataHora);
        return d.getFullYear() === today.getFullYear() &&
               d.getMonth() === today.getMonth() &&
               d.getDate() === today.getDate();
      });
    } else if (days === 9999) {
      return data;
    } else {
      const now = new Date();
      const minDate = new Date(now.getTime() - days * 24 * 60 * 60 * 1000);
      return data.filter((entry: any) => new Date(entry.dataHora) >= minDate);
    }
  }, [data, days]);

  const chartData = {
    labels: filteredData.map((entry: any) => new Date(entry.dataHora).toLocaleString()),
    datasets: [
      {
        label: 'Nível de Humor',
        data: filteredData.map((entry: any) => entry.nivel),
        fill: false,
        borderColor: '#6366f1',
        backgroundColor: '#818cf8',
        tension: 0.2,
        pointRadius: 4,
      },
    ],
  };

  const chartOptions = {
    responsive: true,
    plugins: {
      legend: { display: false },
      tooltip: { enabled: true },
    },
    scales: {
      y: {
        min: 1,
        max: 5,
        title: {
          display: true,
          text: 'Nível de Humor',
          font: { weight: 'bold' as const, size: 14 },
        },
        ticks: { stepSize: 1 }
      },
      x: {
        title: {
          display: true,
          text: 'Data/Hora',
          font: { weight: 'bold' as const, size: 14 },
        },
        ticks: { maxTicksLimit: 8 }
      }
    }
  };

  const handleExportPDF = async () => {
    if (!chartRef.current) return;
    const chartCanvas = chartRef.current.canvas;
    const canvas = await html2canvas(chartCanvas, { backgroundColor: '#fff' });
    const imgData = canvas.toDataURL('image/png');
    const pdf = new jsPDF({ orientation: 'landscape' });
    const pageWidth = pdf.internal.pageSize.getWidth();
    // Nome do app
    pdf.setFontSize(18);
    pdf.text('Mood Tracking', pageWidth / 2, 15, { align: 'center' });
    // Título personalizado
    pdf.setFontSize(14);
    pdf.text(`Histórico de Humor de ${userName || 'usuário'}`, pageWidth / 2, 25, { align: 'center' });
    // Gráfico
    const imgWidth = pageWidth - 20;
    const imgHeight = (canvas.height * imgWidth) / canvas.width;
    pdf.addImage(imgData, 'PNG', 10, 35, imgWidth, imgHeight);
    pdf.save('historico-humor.pdf');
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>
        Histórico de Humor
        <IconButton onClick={onClose} sx={{ position: 'absolute', right: 8, top: 8 }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <Box display="flex" alignItems="center" mb={2} mt={2}>
          <TextField
            select
            label="Período"
            value={days}
            onChange={e => setDays(Number(e.target.value))}
            size="small"
            sx={{ minWidth: 120, mr: 2 }}
          >
            <MenuItem value={0}>Hoje</MenuItem>
            <MenuItem value={7}>Últimos 7 dias</MenuItem>
            <MenuItem value={15}>Últimos 15 dias</MenuItem>
            <MenuItem value={30}>Últimos 30 dias</MenuItem>
            <MenuItem value={90}>Últimos 90 dias</MenuItem>
            <MenuItem value={365}>Últimos 12 meses</MenuItem>
            <MenuItem value={9999}>Tudo</MenuItem>
          </TextField>
          <Button onClick={handleExportPDF} variant="outlined" color="primary" sx={{ ml: 2 }}>
            Exportar PDF
          </Button>
        </Box>
        {loading ? (
          <Box display="flex" justifyContent="center" alignItems="center" minHeight={200}>
            <CircularProgress />
          </Box>
        ) : error ? (
          <Box color="error.main">{error}</Box>
        ) : (
          <Line ref={chartRef} data={chartData} options={chartOptions} />
        )}
      </DialogContent>
    </Dialog>
  );
};

export default MoodHistoryDialog;

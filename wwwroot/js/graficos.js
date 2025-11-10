document.addEventListener("DOMContentLoaded", function () {
  // Paleta de colores personalizada
  const palette = [
    "#4b5cbf",
    "#36A2EB",
    "#FFCE56",
    "#FF6384",
    "#4BC0C0",
    "#9966FF",
    "#FF9F40",
    "#2d357a",
    "#bfc3f7",
  ];

  // Opciones comunes mejoradas
  const commonOptions = {
    plugins: {
      legend: {
        labels: {
          font: { size: 15, family: "'Segoe UI', Arial, sans-serif" },
          color: "#2d357a",
        },
      },
      title: {
        display: false,
      },
      tooltip: {
        backgroundColor: "#4b5cbf",
        titleColor: "#fff",
        bodyColor: "#fff",
        borderColor: "#fff",
        borderWidth: 1,
        padding: 12,
      },
    },
    layout: {
      padding: { left: 16, right: 16, top: 16, bottom: 16 },
    },
    scales: {
      x: {
        grid: { color: "#e7e9fb" },
        ticks: { font: { size: 14 }, color: "#2d357a" },
      },
      y: {
        grid: { color: "#e7e9fb" },
        ticks: { font: { size: 14 }, color: "#2d357a" },
      },
    },
  };

  // === 1. Horas trabajadas por empleado ===
  function crearGraficoHorasTrabajadas(ctx, labels, data) {
    const palette = [
      "#4b5cbf",
      "#36A2EB",
      "#FFCE56",
      "#FF6384",
      "#4BC0C0",
      "#9966FF",
      "#FF9F40",
      "#2d357a",
      "#bfc3f7",
    ];
    const commonOptions = {
      plugins: {
        legend: {
          labels: {
            font: { size: 15, family: "'Segoe UI', Arial, sans-serif" },
            color: "#2d357a",
          },
        },
        title: { display: false },
        tooltip: {
          backgroundColor: "#4b5cbf",
          titleColor: "#fff",
          bodyColor: "#fff",
          borderColor: "#fff",
          borderWidth: 1,
          padding: 12,
        },
      },
      layout: { padding: { left: 16, right: 16, top: 16, bottom: 16 } },
      scales: {
        x: {
          grid: { color: "#e7e9fb" },
          ticks: { font: { size: 14 }, color: "#2d357a" },
        },
        y: {
          grid: { color: "#e7e9fb" },
          ticks: { font: { size: 14 }, color: "#2d357a" },
        },
      },
    };
    return new Chart(ctx, {
      type: "bar",
      data: {
        labels: labels,
        datasets: [
          {
            label: "Horas trabajadas",
            data: data,
            backgroundColor: palette,
            borderRadius: 10,
            borderSkipped: false,
          },
        ],
      },
      options: {
        ...commonOptions,
        plugins: {
          ...commonOptions.plugins,
          title: {
            display: true,
            text: "Horas trabajadas por empleado",
            font: { size: 20, weight: "bold" },
            color: "#4b5cbf",
            padding: { top: 10, bottom: 20 },
          },
          legend: { display: false },
          tooltip: commonOptions.plugins.tooltip,
        },
        scales: {
          x: { display: false },
          y: commonOptions.scales.y,
        },
      },
    });
  }
  window.crearGraficoHorasTrabajadas = crearGraficoHorasTrabajadas;

  try {
    const chartElement = document.getElementById("chartAsistencia");
    if (
      chartElement &&
      window.horasTrabajadasLabels &&
      window.horasTrabajadasData
    ) {
      // Usar la función global y guardar la instancia
      window.chartAsistenciaInstance = crearGraficoHorasTrabajadas(
        chartElement.getContext("2d"),
        window.horasTrabajadasLabels,
        window.horasTrabajadasData
      );
    }
  } catch (error) {
    console.error("Error al crear gráfica de horas trabajadas:", error);
  }

  try {
    const chartElement = document.getElementById("chartSede");
    if (chartElement && window.sedesLabels && window.sedesData) {
      new Chart(chartElement, {
        type: "doughnut",
        data: {
          labels: window.sedesLabels,
          datasets: [
            {
              label: "Empleados por sede",
              data: window.sedesData,
              backgroundColor: palette,
              borderColor: "#fff",
              borderWidth: 2,
            },
          ],
        },
        options: {
          plugins: {
            ...commonOptions.plugins,
            title: {
              display: true,
              text: "Empleados por Sede",
              font: { size: 20, weight: "bold" },
              color: "#4b5cbf",
              padding: { top: 10, bottom: 20 },
            },
          },
          cutout: "65%",
          layout: commonOptions.layout,
        },
      });
    }

    const filtroSedeUsuarios = document.getElementById("filtroSedeUsuarios");
    const usuariosPorSedeDiv = document.getElementById("usuariosPorSede");
    if (filtroSedeUsuarios && usuariosPorSedeDiv && window.sedesUsuariosRaw) {
      filtroSedeUsuarios.addEventListener("change", function () {
        const sede = this.value;
        usuariosPorSedeDiv.innerHTML = "";
        if (sede) {
          const sedeObj = window.sedesUsuariosRaw.find((x) => x.sede === sede);
          if (sedeObj && sedeObj.usuarios.length > 0) {
            usuariosPorSedeDiv.innerHTML =
              "<strong>Usuarios en " +
              sede +
              ":</strong><ul class='list-group mt-2'>" +
              sedeObj.usuarios
                .map((u) => "<li class='list-group-item py-1'>" + u + "</li>")
                .join("") +
              "</ul>";
          } else {
            usuariosPorSedeDiv.innerHTML =
              "<em>No hay usuarios en esta sede.</em>";
          }
        }
      });
    }
  } catch (error) {
    console.error("Error al crear gráfica de empleados por sede:", error);
  }

  try {
    const chartElement = document.getElementById("chartTurno");
    if (chartElement && window.turnosLabels && window.turnosData) {
      new Chart(chartElement, {
        type: "bar",
        data: {
          labels: window.turnosLabels,
          datasets: [
            {
              label: "Empleados por turno",
              data: window.turnosData,
              backgroundColor: palette,
              borderRadius: 10,
              borderSkipped: false,
            },
          ],
        },
        options: {
          ...commonOptions,
          plugins: {
            ...commonOptions.plugins,
            title: {
              display: true,
              text: "Empleados por Turno",
              font: { size: 20, weight: "bold" },
              color: "#4b5cbf",
              padding: { top: 10, bottom: 20 },
            },
          },
          scales: {
            x: {
              ...commonOptions.scales.x,
              title: { display: false },
              ticks: { display: false }, // Oculta las letras debajo del gráfico
            },
            y: {
              ...commonOptions.scales.y,
              ticks: { display: false }, // Oculta los números a la izquierda
            },
          },
        },
      });
    }
  } catch (error) {
    console.error("Error al crear gráfica de empleados por turno:", error);
  }

  // === 4. Supervisores con más empleados ===
  try {
    const chartElement = document.getElementById("chartSupervisores");
    if (chartElement && window.supervisoresLabels && window.supervisoresData) {
      new Chart(chartElement, {
        type: "horizontalBar" in Chart.defaults ? "horizontalBar" : "bar",
        data: {
          labels: window.supervisoresLabels,
          datasets: [
            {
              label: "Cantidad de empleados supervisados",
              data: window.supervisoresData,
              backgroundColor: palette,
              borderRadius: 10,
              borderSkipped: false,
            },
          ],
        },
        options: {
          ...commonOptions,
          indexAxis: "y",
          plugins: {
            ...commonOptions.plugins,
            title: {
              display: true,
              text: "Supervisores con más empleados",
              font: { size: 20, weight: "bold" },
              color: "#4b5cbf",
              padding: { top: 10, bottom: 20 },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error("Error al crear gráfica de supervisores:", error);
  }

  // === 5. Horas trabajadas por semana ===
  try {
    const chartElement = document.getElementById("chartHorasSemana");
    if (chartElement && window.semanasLabels && window.semanasData) {
      new Chart(chartElement, {
        type: "line",
        data: {
          labels: window.semanasLabels,
          datasets: [
            {
              label: "Total horas por semana",
              data: window.semanasData,
              borderColor: "#4b5cbf",
              backgroundColor: "rgba(75, 92, 191, 0.13)",
              tension: 0.4,
              pointBackgroundColor: "#4b5cbf",
              pointRadius: 6,
              pointHoverRadius: 9,
              fill: true,
            },
          ],
        },
        options: {
          ...commonOptions,
          plugins: {
            ...commonOptions.plugins,
            title: {
              display: true,
              text: "Horas trabajadas por semana",
              font: { size: 20, weight: "bold" },
              color: "#4b5cbf",
              padding: { top: 10, bottom: 20 },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error("Error al crear gráfica de horas por semana:", error);
  }

  // === 6. Pagos mensuales ===
  try {
    const chartElement = document.getElementById("graficoPagosMensuales");
    if (
      chartElement &&
      window.mesesPagosLabels &&
      window.mesesPagosData &&
      window.mesesPagosLabels.length > 0 &&
      window.mesesPagosData.length > 0
    ) {
      new Chart(chartElement, {
        type: "bar",
        data: {
          labels: window.mesesPagosLabels,
          datasets: [
            {
              label: "Total a pagar por mes",
              data: window.mesesPagosData,
              backgroundColor: palette,
              borderColor: "#4b5cbf",
              borderWidth: 1,
              borderRadius: 10,
              borderSkipped: false,
            },
          ],
        },
        options: {
          ...commonOptions,
          plugins: {
            ...commonOptions.plugins,
            title: {
              display: true,
              text: "Pagos Mensuales",
              font: { size: 20, weight: "bold" },
              color: "#4b5cbf",
              padding: { top: 10, bottom: 20 },
            },
          },
          scales: {
            y: {
              beginAtZero: true,
              title: {
                display: true,
                text: "Monto en S/.",
              },
              ticks: { font: { size: 14 }, color: "#2d357a" },
            },
            x: {
              title: {
                display: true,
                text: "Mes/Año",
              },
              ticks: { font: { size: 14 }, color: "#2d357a" },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error("Error al crear gráfica de pagos mensuales:", error);
  }

  // === 7. Tipos de empleado ===
  try {
    const chartElement = document.getElementById("chartTipoEmpleado");
    if (
      chartElement &&
      window.tiposEmpleadoLabels &&
      window.tiposEmpleadoData
    ) {
      new Chart(chartElement, {
        type: "bar",
        data: {
          labels: window.tiposEmpleadoLabels,
          datasets: [
            {
              label: "Promedio de Evaluación",
              data: window.tiposEmpleadoData,
              backgroundColor: palette,
              borderRadius: 10,
              borderSkipped: false,
            },
          ],
        },
        options: {
          ...commonOptions,
          plugins: {
            ...commonOptions.plugins,
            title: {
              display: true,
              text: "Promedio de Evaluación por Tipo de Empleado",
              font: { size: 20, weight: "bold" },
              color: "#4b5cbf",
              padding: { top: 10, bottom: 20 },
            },
          },
          scales: {
            y: {
              beginAtZero: true,
              max: 4,
              ticks: {
                stepSize: 1,
                font: { size: 14 },
                color: "#2d357a",
              },
            },
            x: {
              ticks: { font: { size: 14 }, color: "#2d357a" },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error("Error al crear gráfica de tipos de empleado:", error);
  }

  // === 8. Criterios de evaluación ===
  try {
    const chartElement = document.getElementById("chartCriterios");
    if (chartElement && window.criteriosLabels && window.criteriosData) {
      new Chart(chartElement, {
        type: "radar",
        data: {
          labels: window.criteriosLabels,
          datasets: [
            {
              label: "Promedio por Criterio",
              data: window.criteriosData,
              backgroundColor: "rgba(75, 92, 191, 0.13)",
              borderColor: "#4b5cbf",
              pointBackgroundColor: "#4b5cbf",
              pointBorderColor: "#fff",
              pointRadius: 6,
              pointHoverRadius: 9,
            },
          ],
        },
        options: {
          plugins: {
            ...commonOptions.plugins,
            title: {
              display: true,
              text: "Promedio por Criterio de Evaluación",
              font: { size: 20, weight: "bold" },
              color: "#4b5cbf",
              padding: { top: 10, bottom: 20 },
            },
          },
          scales: {
            r: {
              min: 0,
              max: 4,
              ticks: {
                stepSize: 1,
                font: { size: 14 },
                color: "#2d357a",
              },
              grid: { color: "#e7e9fb" },
              pointLabels: { font: { size: 14 }, color: "#2d357a" },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error("Error al crear gráfica de criterios:", error);
  }
});

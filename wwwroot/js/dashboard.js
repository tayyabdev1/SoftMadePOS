window.renderSalesChart = (canvasId, labels, data) => {
    const ctx = document.getElementById(canvasId).getContext('2d');

    // Destroy existing chart if it exists (to prevent glitches on refresh)
    if (window.mySalesChart) {
        window.mySalesChart.destroy();
    }

    window.mySalesChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Daily Sales (Rs.)',
                data: data,
                borderColor: '#007bff',
                backgroundColor: 'rgba(0, 123, 255, 0.1)',
                borderWidth: 2,
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: { beginAtZero: true }
            }
        }
    });
};
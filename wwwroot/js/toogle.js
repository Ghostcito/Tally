document.addEventListener('DOMContentLoaded', function() {
    const sidebar = document.getElementById('sidebar');
    const toggleBtn = document.getElementById('sidebarToggle');
    const sidebarOverlay = document.querySelector('.sidebar-overlay');
            
    // Estado inicial desde localStorage
    const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (isCollapsed) {
        sidebar.classList.add('collapsed');
    }
            
    // Manejar el toggle del sidebar
    toggleBtn?.addEventListener('click', function() {
        const nowCollapsed = sidebar.classList.toggle('collapsed');
        localStorage.setItem('sidebarCollapsed', nowCollapsed);
                
        // En móviles, mostrar/ocultar sidebar completo
        if (window.innerWidth <= 768) {
            sidebar.classList.toggle('show');
        }
    });
            
    // Cerrar sidebar al hacer clic en overlay (móviles)
    sidebarOverlay?.addEventListener('click', function() {
        sidebar.classList.remove('show');
    });
            
    // Manejar responsive
    function handleResponsive() {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('show');
            sidebarOverlay.style.display = 'none';
        }
    }
            
    window.addEventListener('resize', handleResponsive);
    handleResponsive(); // Ejecutar al cargar
});
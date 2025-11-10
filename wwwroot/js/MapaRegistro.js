// Configuración inicial del mapa
const map = L.map('map').setView([-16.398833, -71.536970], 12); // Centro inicial en Arequipa, Perú

// Capa base de OpenStreetMap
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

// Variables globales
let marker;
let lastReverseGeocode = 0;

// Función para geocodificación inversa (obtener dirección desde coordenadas)
async function reverseGeocode(latlng) {
    const now = Date.now();
    if (now - lastReverseGeocode < 1000) return null; // Esperar 1 segundo entre peticiones
    
    lastReverseGeocode = now;
    
    try {
        const response = await fetch(
            `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${latlng.lat}&lon=${latlng.lng}`, 
            {
                headers: {
                    'User-Agent': 'SistemaControlLimpieza/1.0 (contacto@tudominio.com)'
                }
            }
        );
        
        if (!response.ok) throw new Error('Error en la petición');
        
        const data = await response.json();
        return data;
        
    } catch (error) {
        console.error('Error en geocodificación inversa:', error);
        return null;
    }
}

// Función para actualizar el formulario con los datos de dirección
function updateAddressFields(data) {
    const address = data.address || {};
    
    // Dirección principal (primer elemento del display_name)
    document.getElementById('Direccion_local').value = data.display_name?.split(',')[0] || '';
    
    // Ciudad (prioriza city, luego town, luego village)
    document.getElementById('Ciudad').value = 
        address.city  || address.state;
    
    // Provincia/Estado
    document.getElementById('Provincia').value = address.state || address.region || '';
}

// Función principal para actualizar el marcador
async function updateMarker(latlng) {
    // Eliminar marcador existente
    if (marker) {
        map.removeLayer(marker);
    }
    
    // Crear nuevo marcador
    marker = L.marker(latlng, {
        draggable: true
    }).addTo(map);
    
    // Actualizar campos de coordenadas
    document.getElementById('latitud-input').value = latlng.lat.toFixed(6);
    document.getElementById('longitud-input').value = latlng.lng.toFixed(6);
    
    // Obtener datos de dirección (geocodificación inversa)
    const geoData = await reverseGeocode(latlng);
    
    if (geoData) {
        updateAddressFields(geoData);
        
        // Actualizar popup con información detallada
        marker.bindPopup(`
            <b>Ubicación seleccionada</b><br>
            <small>${geoData.display_name || 'Dirección no disponible'}</small><br>
            Lat: ${latlng.lat.toFixed(6)}<br>
            Lng: ${latlng.lng.toFixed(6)}
        `).openPopup();
    } else {
        // Popup básico si falla la geocodificación
        marker.bindPopup(`
            <b>Ubicación seleccionada</b><br>
            Lat: ${latlng.lat.toFixed(6)}<br>
            Lng: ${latlng.lng.toFixed(6)}
        `).openPopup();
    }
    
    // Evento para arrastrar el marcador
    marker.on('dragend', function(e) {
        updateMarker(e.target.getLatLng());
    });
}

// Evento click en el mapa
map.on('click', function(e) {
    updateMarker(e.latlng);
});

// Configurar geocoder (buscador integrado)
const geocoder = L.Control.geocoder({
    defaultMarkGeocode: false,
    position: 'topright',
    geocoder: L.Control.Geocoder.nominatim({
        geocodingQueryParams: {
            'accept-language': 'es',
            countrycodes: 'pe' // Priorizar resultados en Perú
        }
    })
}).on('markgeocode', function(e) {
    const result = e.geocode;
    map.setView(result.center, 16);
    updateMarker(result.center);
}).addTo(map);

// Buscador manual alternativo
document.getElementById('search-button')?.addEventListener('click', async function() {
    const address = document.getElementById('address-search').value;
    if (!address) return;
    
    try {
        const response = await fetch(
            `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}&accept-language=es&countrycodes=pe`,
            {
                headers: {
                    'User-Agent': 'SistemaControlLimpieza/1.0 (contacto@tudominio.com)'
                }
            }
        );
        
        const data = await response.json();
        
        if (data.length > 0) {
            const firstResult = data[0];
            const latlng = L.latLng(firstResult.lat, firstResult.lon);
            map.setView(latlng, 16);
            updateMarker(latlng);
        } else {
            alert('Dirección no encontrada');
        }
    } catch (error) {
        console.error('Error en búsqueda:', error);
        alert('Error al buscar la dirección');
    }
});
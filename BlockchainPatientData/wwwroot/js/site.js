// ===================================
// HMS Blockchain Project - JavaScript
// ===================================

// Document ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('🏥 HMS Blockchain System Loaded');
    
    // Add fade-in animation to main content
    const mainContent = document.querySelector('main');
    if (mainContent) {
        mainContent.classList.add('fade-in');
    }
    
    // Initialize tooltips if Bootstrap is available
    if (typeof bootstrap !== 'undefined') {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
    
    // Form validation enhancement
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!form.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });
    
    // Hash copy functionality
    const hashElements = document.querySelectorAll('code');
    hashElements.forEach(element => {
        element.style.cursor = 'pointer';
        element.title = 'Click to copy';
        element.addEventListener('click', function() {
            copyToClipboard(this.textContent);
            showNotification('Hash copied to clipboard!', 'success');
        });
    });
});

// Copy to clipboard function
function copyToClipboard(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text).then(() => {
            console.log('Copied to clipboard');
        }).catch(err => {
            console.error('Failed to copy:', err);
        });
    } else {
        // Fallback for older browsers
        const textArea = document.createElement('textarea');
        textArea.value = text;
        document.body.appendChild(textArea);
        textArea.select();
        try {
            document.execCommand('copy');
        } catch (err) {
            console.error('Failed to copy:', err);
        }
        document.body.removeChild(textArea);
    }
}

// Show notification
function showNotification(message, type = 'info') {
    const notificationDiv = document.createElement('div');
    notificationDiv.className = `alert alert-${type} position-fixed top-0 end-0 m-3`;
    notificationDiv.style.zIndex = '9999';
    notificationDiv.textContent = message;
    
    document.body.appendChild(notificationDiv);
    
    setTimeout(() => {
        notificationDiv.remove();
    }, 3000);
}

// Blockchain hash visualizer
function visualizeHash(hash) {
    if (!hash) return;
    
    console.log('Blockchain Hash:', hash);
    console.log('Hash Length:', hash.length);
    console.log('First 10 chars:', hash.substring(0, 10));
    console.log('Last 10 chars:', hash.substring(hash.length - 10));
}

// Format amount as currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-PK', {
        style: 'currency',
        currency: 'PKR'
    }).format(amount);
}

// Validate patient ID
function validatePatientId(id) {
    return id > 0 && Number.isInteger(id);
}

// Validate amount
function validateAmount(amount) {
    return amount > 0 && !isNaN(amount);
}

// Generate random color for NFT visualization
function generateNFTColor(nftId) {
    let hash = 0;
    for (let i = 0; i < nftId.length; i++) {
        hash = nftId.charCodeAt(i) + ((hash << 5) - hash);
    }
    const color = Math.floor(Math.abs((Math.sin(hash) * 16777215) % 1) * 16777215);
    return '#' + color.toString(16).padStart(6, '0');
}

// Animate number counter
function animateValue(element, start, end, duration) {
    if (!element) return;
    
    let startTimestamp = null;
    const step = (timestamp) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);
        element.textContent = Math.floor(progress * (end - start) + start);
        if (progress < 1) {
            window.requestAnimationFrame(step);
        }
    };
    window.requestAnimationFrame(step);
}

// Check blockchain status (simulation)
function checkBlockchainStatus() {
    const status = {
        active: true,
        blocks: Math.floor(Math.random() * 1000) + 100,
        lastHash: generateRandomHash(),
        timestamp: new Date().toISOString()
    };
    
    console.log('Blockchain Status:', status);
    return status;
}

// Generate random hash (for demo purposes)
function generateRandomHash() {
    const chars = '0123456789ABCDEF';
    let hash = '';
    for (let i = 0; i < 64; i++) {
        hash += chars[Math.floor(Math.random() * 16)];
    }
    return hash;
}

// Fraud risk calculator
function calculateFraudRisk(amount, frequency) {
    let risk = 0;
    
    if (amount > 50000) risk += 40;
    else if (amount > 20000) risk += 20;
    
    if (frequency > 5) risk += 30;
    else if (frequency > 3) risk += 15;
    
    if (amount % 1000 === 0 && amount > 10000) risk += 15;
    
    return Math.min(risk, 100);
}

// Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-PK', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Console branding
console.log('%c🏥 HMS Blockchain System', 'color: #0d6efd; font-size: 20px; font-weight: bold;');
console.log('%cSecured with SHA256 Blockchain Technology', 'color: #198754; font-size: 14px;');
console.log('%c© 2025 Final Year Project', 'color: #6c757d; font-size: 12px;');

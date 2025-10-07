// Security validation and XSS prevention utilities
(function() {
    'use strict';

    // XSS prevention - sanitize input
    function sanitizeInput(input) {
        if (typeof input !== 'string') return input;
        
        // Remove potentially dangerous characters and scripts
        return input
            .replace(/[<>]/g, '') // Remove < and >
            .replace(/javascript:/gi, '') // Remove javascript: protocol
            .replace(/on\w+\s*=/gi, '') // Remove event handlers
            .replace(/script/gi, '') // Remove script tags
            .replace(/iframe/gi, '') // Remove iframe tags
            .replace(/object/gi, '') // Remove object tags
            .replace(/embed/gi, '') // Remove embed tags
            .replace(/link/gi, '') // Remove link tags
            .replace(/meta/gi, '') // Remove meta tags
            .trim();
    }

    // Validate email format
    function validateEmail(email) {
        const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$/;
        return emailRegex.test(email);
    }

    // Validate phone number
    function validatePhone(phone) {
        const phoneRegex = /^\+?[0-9\s\-()]{7,15}$/;
        return phoneRegex.test(phone);
    }

    // Validate name (letters, spaces, hyphens, apostrophes only)
    function validateName(name) {
        const nameRegex = /^[A-Za-zÀ-ÖØ-öø-ÿ' -]+$/;
        return nameRegex.test(name) && name.length >= 2 && name.length <= 50;
    }

    // Validate company name
    function validateCompany(company) {
        if (!company) return true; // Optional field
        const companyRegex = /^[A-Za-z0-9À-ÖØ-öø-ÿ' -&.,]+$/;
        return companyRegex.test(company) && company.length <= 100;
    }

    // Validate message content
    function validateMessage(message) {
        if (!message) return true; // Optional field
        const messageRegex = /^[A-Za-z0-9À-ÖØ-öø-ÿ' -&.,!?@#$%^&*()_+={}[\]|\\:;""'<>,./~`\s]*$/;
        return messageRegex.test(message) && message.length <= 1000;
    }

    // Real-time form validation
    function setupFormValidation() {
        const forms = document.querySelectorAll('form[novalidate]');
        
        forms.forEach(form => {
            const inputs = form.querySelectorAll('input, textarea, select');
            
            inputs.forEach(input => {
                // Add event listeners for real-time validation
                input.addEventListener('blur', function() {
                    validateField(this);
                });
                
                input.addEventListener('input', function() {
                    // Clear previous error styling
                    this.classList.remove('is-invalid');
                    const errorMsg = this.parentNode.querySelector('.invalid-feedback');
                    if (errorMsg) {
                        errorMsg.remove();
                    }
                });
            });
            
            // Form submission validation
            form.addEventListener('submit', function(e) {
                let isValid = true;
                
                // Validate each field
                inputs.forEach(input => {
                    if (!validateField(input)) {
                        isValid = false;
                    }
                });
                
                if (!isValid) {
                    e.preventDefault();
                    showErrorMessage('Please correct the errors and try again.');
                } else {
                    // Sanitize all inputs before submission
                    inputs.forEach(input => {
                        if (input.type !== 'password' && input.type !== 'file') {
                            input.value = sanitizeInput(input.value);
                        }
                    });
                    
                    // Allow natural submission (no preventDefault)
                }
            });
        });
    }

    // Validate individual field
    function validateField(field) {
        const value = field.value.trim();
        const fieldName = field.name || field.placeholder || 'Field';
        let isValid = true;
        let errorMessage = '';

        // Required field validation
        if (field.hasAttribute('required') && !value) {
            isValid = false;
            errorMessage = `${fieldName} is required.`;
        }
        // Email validation
        else if (field.type === 'email' && value && !validateEmail(value)) {
            isValid = false;
            errorMessage = 'Please enter a valid email address.';
        }
        // Phone validation
        else if (field.type === 'tel' && value && !validatePhone(value)) {
            isValid = false;
            errorMessage = 'Please enter a valid phone number.';
        }
        // Name validation
        else if (field.placeholder && (field.placeholder.includes('Name') || field.placeholder.includes('Surname')) && value && !validateName(value)) {
            isValid = false;
            errorMessage = 'Please enter a valid name (letters, spaces, hyphens, and apostrophes only).';
        }
        // Company validation
        else if (field.placeholder && field.placeholder.includes('Company') && value && !validateCompany(value)) {
            isValid = false;
            errorMessage = 'Please enter a valid company name.';
        }
        // Message validation
        else if (field.tagName === 'TEXTAREA' && value && !validateMessage(value)) {
            isValid = false;
            errorMessage = 'Please enter a valid message (up to 1000 characters).';
        }
        // Select validation
        else if (field.tagName === 'SELECT' && field.hasAttribute('required')) {
            const disabledOpt = field.querySelector('option[disabled]');
            const placeholderValues = new Set(['', disabledOpt ? disabledOpt.value : '']);
            if (!value || placeholderValues.has(value)) {
                isValid = false;
                errorMessage = 'Please select an option.';
            }
        }

        // Apply validation styling
        if (isValid) {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid');
        } else {
            field.classList.remove('is-valid');
            field.classList.add('is-invalid');
            
            // Show error message
            showFieldError(field, errorMessage);
        }

        return isValid;
    }

    // Show field error message
    function showFieldError(field, message) {
        // Remove existing error message
        const existingError = field.parentNode.querySelector('.invalid-feedback');
        if (existingError) {
            existingError.remove();
        }

        // Add new error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        errorDiv.textContent = message;
        field.parentNode.appendChild(errorDiv);
    }

    // Show success message
    function showSuccessMessage(message) {
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-success alert-dismissible fade show';
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        // Insert at the top of the form
        const form = document.querySelector('form');
        if (form) {
            form.parentNode.insertBefore(alertDiv, form);
        }
    }

    // Show error message
    function showErrorMessage(message) {
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger alert-dismissible fade show';
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        // Insert at the top of the form
        const form = document.querySelector('form');
        if (form) {
            form.parentNode.insertBefore(alertDiv, form);
        }
    }

    // Initialize security features when DOM is loaded
    document.addEventListener('DOMContentLoaded', function() {
        setupFormValidation();
        
        // Add clickjacking protection check
        if (window.top !== window.self) {
            // If the page is in a frame, redirect to prevent clickjacking
            window.top.location = window.self.location;
        }
        
        // Disable right-click context menu (optional security measure)
        document.addEventListener('contextmenu', function(e) {
            e.preventDefault();
        });
        
        // Disable F12, Ctrl+Shift+I, Ctrl+U (optional security measures)
        document.addEventListener('keydown', function(e) {
            if (e.key === 'F12' || 
                (e.ctrlKey && e.shiftKey && e.key === 'I') ||
                (e.ctrlKey && e.key === 'u')) {
                e.preventDefault();
            }
        });
    });

    // Expose sanitize function globally for use in other scripts
    window.sanitizeInput = sanitizeInput;
})();})();
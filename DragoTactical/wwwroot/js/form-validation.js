// Form validation for all contact forms
(function() {
    'use strict';

    // Initialize validation for all forms
    function initializeFormValidation() {
        const forms = document.querySelectorAll('form[id$="ContactForm"], form[id="contactForm"]');
        forms.forEach(form => {
            setupFormValidation(form);
        });
    }

    function setupFormValidation(form) {
        const sanitize = (v) => v.replace(/[<>]/g, '').trim();
        
        // Rate limiting for security
        let submissionAttempts = 0;
        const maxAttempts = 5;
        const rateLimitWindow = 60000; // 1 minute
        let lastSubmissionTime = 0;

        const validators = {
            FirstName: (v) => {
                if (!v || v.length < 2) return { valid: false, message: 'First name must be at least 2 characters.' };
                if (v.length > 50) return { valid: false, message: 'First name cannot exceed 50 characters.' };
                if (!/^[A-Za-z\s]+$/.test(v)) return { valid: false, message: 'First name must contain only letters and spaces.' };
                return { valid: true };
            },
            LastName: (v) => {
                if (!v || v.length < 2) return { valid: false, message: 'Last name must be at least 2 characters.' };
                if (v.length > 50) return { valid: false, message: 'Last name cannot exceed 50 characters.' };
                if (!/^[A-Za-z\s]+$/.test(v)) return { valid: false, message: 'Last name must contain only letters and spaces.' };
                return { valid: true };
            },
            Email: (v) => {
                if (!v) return { valid: false, message: 'Email address is required.' };
                if (!v.includes('@')) return { valid: false, message: 'Email must contain @ symbol.' };
                var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(v)) return { valid: false, message: 'Email must be in valid format (e.g., user@example.com).' };
                if (v.length > 254) return { valid: false, message: 'Email cannot exceed 254 characters.' };
                return { valid: true };
            },
            PhoneNumber: (v) => {
                if (!v) return { valid: false, message: 'Phone number is required.' };
                if (v.length < 10) return { valid: false, message: 'Phone number must be at least 10 digits.' };
                if (v.length > 15) return { valid: false, message: 'Phone number cannot exceed 15 digits.' };
                if (!/^\+?[0-9\s\-]+$/.test(v)) return { valid: false, message: 'Phone must be numbers only (+ allowed at start).' };
                return { valid: true };
            },
            CompanyName: (v) => {
                if (v && v.length > 100) return { valid: false, message: 'Company name cannot exceed 100 characters.' };
                if (v && /[<>]/.test(v)) return { valid: false, message: 'Company name cannot contain < or > characters.' };
                return { valid: true };
            },
            Location: (v) => {
                if (!v) return { valid: false, message: 'Please select your location from the dropdown.' };
                return { valid: true };
            },
            ServiceId: (v) => {
                if (!v) return { valid: false, message: 'Please select a service from the dropdown.' };
                return { valid: true };
            },
            Message: (v) => {
                if (!v) return { valid: false, message: 'Message is required.' };
                if (v.length < 10) return { valid: false, message: 'Message must be at least 10 characters.' };
                if (v.length > 1000) return { valid: false, message: 'Message cannot exceed 1000 characters.' };
                if (/[<>]/.test(v)) return { valid: false, message: 'Message cannot contain < or > characters.' };
                return { valid: true };
            }
        };

        function setInvalid(input, message) {
            input.classList.add('is-invalid');
            const feedback = input.parentElement.querySelector('.invalid-feedback');
            if (feedback) {
                feedback.textContent = message;
                feedback.style.visibility = 'visible';
            }
        }

        function clearInvalid(input) {
            input.classList.remove('is-invalid');
            const feedback = input.parentElement.querySelector('.invalid-feedback');
            if (feedback) {
                feedback.style.visibility = 'hidden';
            }
        }

        function showSecurityError(message) {
            const alertDiv = document.createElement('div');
            alertDiv.className = 'alert alert-danger';
            alertDiv.innerHTML = '<strong>Security Alert:</strong> ' + message;
            form.parentNode.insertBefore(alertDiv, form);
            setTimeout(() => alertDiv.remove(), 5000);
        }

        // Add real-time validation
        const inputs = form.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            input.addEventListener('input', function(e) {
                const name = this.getAttribute('name');
                const val = sanitize(this.value);
                this.value = val;
                if (validators[name]) {
                    const result = validators[name](val);
                    if (result.valid) {
                        clearInvalid(this);
                    } else {
                        setInvalid(this, result.message);
                    }
                }
            });
            
            // Also validate on blur (when user leaves field)
            input.addEventListener('blur', function(e) {
                const name = this.getAttribute('name');
                const val = sanitize(this.value);
                this.value = val;
                if (validators[name]) {
                    const result = validators[name](val);
                    if (result.valid) {
                        clearInvalid(this);
                    } else {
                        setInvalid(this, result.message);
                    }
                }
            });
        });

        form.addEventListener('submit', function (e) {
            e.preventDefault();
            
            // Rate limiting check
            const now = Date.now();
            if (now - lastSubmissionTime < rateLimitWindow) {
                submissionAttempts++;
                if (submissionAttempts >= maxAttempts) {
                    showSecurityError('Too many submission attempts. Please wait 1 minute before trying again.');
                    return;
                }
            } else {
                submissionAttempts = 1;
                lastSubmissionTime = now;
            }
            
            let allValid = true;

            const fields = form.querySelectorAll('input[name], textarea[name], select[name]');
            fields.forEach(field => {
                field.value = sanitize(field.value);
                clearInvalid(field);

                const name = field.getAttribute('name');
                const val = field.value;
                
                if (validators[name]) {
                    const result = validators[name](val);
                    if (!result.valid) {
                        allValid = false;
                        setInvalid(field, result.message);
                    }
                }
            });

            if (allValid) {
                form.submit();
            }
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeFormValidation);
    } else {
        initializeFormValidation();
    }
})();

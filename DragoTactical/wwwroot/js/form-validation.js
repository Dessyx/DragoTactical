// Form validation for all contact forms
// Version: 2.1 - Fixed browser tooltip override
(function() {
    'use strict';

    // Initialize validation for all forms
    function initializeFormValidation() {
        const forms = document.querySelectorAll('form[id$="ContactForm"], form[id="contactForm"], form.phy-form, form.dt-cyber-contact-form, form.dt-contact-form');
        forms.forEach(form => {
            setupFormValidation(form);
        });
    }

    function setupFormValidation(form) {
        const sanitize = (v) => {
            if (!v) return v;
            return v.replace(/[<>]/g, '').trim();
        };
        const sanitizeMessage = (v) => {
            if (!v) return v;
            return v.replace(/[<>]/g, '');
        };
        
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
                if (!v) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
                const digitsOnly = v.replace(/[\s\-]/g, '');
                if (digitsOnly.length < 10) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
                if (digitsOnly.length > 15) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
                if (!/^\+?[0-9\s\-]+$/.test(v)) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
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
                if (!v) return { valid: false, message: 'Please enter a minimum of 10 characters.' };
                if (v.length < 10) return { valid: false, message: 'Please enter a minimum of 10 characters.' };
                if (v.length > 15) return { valid: false, message: 'Please enter a minimum of 10 characters.' };
                return { valid: true };
            }
        };

        function setInvalid(input, message) {
            input.classList.add('is-invalid');
            
            // Find feedback element - try multiple strategies
            let feedback = input.parentElement.querySelector('.invalid-feedback');
            if (!feedback) {
                const wrapper = input.closest('.col-12');
                if (wrapper) {
                    feedback = wrapper.querySelector('.invalid-feedback');
                }
            }
            if (!feedback) {
                feedback = input.parentElement.parentElement.querySelector('.invalid-feedback');
            }
            
            if (feedback) {
                feedback.textContent = message;
                feedback.style.visibility = 'visible';
                feedback.style.display = 'block';
                feedback.style.opacity = '1';
                feedback.classList.add('d-block');
                feedback.classList.remove('d-none');
            }
            input.setCustomValidity(message);
        }

        function clearInvalid(input) {
            input.classList.remove('is-invalid');
            const feedback = input.parentElement.querySelector('.invalid-feedback');
            if (feedback) {
                feedback.style.visibility = 'hidden';
            } else {
                // Fallback: try to find feedback in col-12 wrapper
                const wrapper = input.closest('.col-12');
                if (wrapper) {
                    const fb = wrapper.querySelector('.invalid-feedback');
                    if (fb) {
                        fb.style.visibility = 'hidden';
                    }
                }
            }
            input.setCustomValidity('');
        }

        function showSecurityError(message) {
            const alertDiv = document.createElement('div');
            alertDiv.className = 'alert alert-danger';
            alertDiv.innerHTML = '<strong>Security Alert:</strong> ' + message;
            form.parentNode.insertBefore(alertDiv, form);
            setTimeout(() => alertDiv.remove(), 5000);
        }

        // Prevent browser's default validation messages - catch all invalid events
        form.addEventListener('invalid', function(e) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();
            const field = e.target;
            const name = field.getAttribute('name');
            if (validators[name]) {
                const val = field.value;
                const result = validators[name](val);
                if (!result.valid) {
                    setInvalid(field, result.message);
                }
            }
            return false;
        }, true);

        // Add real-time validation
        const inputs = form.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            // Remove title attribute to prevent browser tooltips
            if (input.hasAttribute('title')) {
                input.setAttribute('data-original-title', input.getAttribute('title'));
                input.removeAttribute('title');
            }

            // Prevent browser's native validation for individual fields
            input.addEventListener('invalid', function(e) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();
                const name = this.getAttribute('name');
                if (validators[name]) {
                    const val = this.value;
                    const result = validators[name](val);
                    if (!result.valid) {
                        setInvalid(this, result.message);
                    }
                }
                return false;
            }, true);

            input.addEventListener('input', function(e) {
                const name = this.getAttribute('name');
                // For Message field, don't sanitize during typing to allow spaces
                if (name === 'Message') {
                    const val = this.value;
                    // Always validate message field on every keystroke
                    if (validators[name]) {
                        const result = validators[name](val);
                        if (!result.valid) {
                            setInvalid(this, result.message);
                            this.setCustomValidity(result.message);
                        } else {
                            clearInvalid(this);
                            this.setCustomValidity('');
                        }
                    } else if (val.length < 10) {
                        // Fallback validation if validator not found
                        setInvalid(this, 'Please enter a minimum of 10 characters.');
                        this.setCustomValidity('Please enter a minimum of 10 characters.');
                    } else {
                        clearInvalid(this);
                        this.setCustomValidity('');
                    }
                } else {
                    const val = sanitize(this.value);
                    this.value = val;
                    if (validators[name]) {
                        const result = validators[name](val);
                        if (result.valid) {
                            clearInvalid(this);
                            this.setCustomValidity('');
                        } else {
                            setInvalid(this, result.message);
                            this.setCustomValidity(result.message);
                        }
                    }
                }
            });
            
            // Also validate on blur (when user leaves field)
            input.addEventListener('blur', function(e) {
                const name = this.getAttribute('name');
                // For Message field, only remove dangerous chars, don't trim
                if (name === 'Message') {
                    const val = sanitizeMessage(this.value);
                    this.value = val;
                    if (validators[name]) {
                        const result = validators[name](val);
                        if (result.valid) {
                            clearInvalid(this);
                            this.setCustomValidity('');
                        } else {
                            setInvalid(this, result.message);
                            this.setCustomValidity(result.message);
                        }
                    }
                } else {
                    const val = sanitize(this.value);
                    this.value = val;
                    if (validators[name]) {
                        const result = validators[name](val);
                        if (result.valid) {
                            clearInvalid(this);
                            this.setCustomValidity('');
                        } else {
                            setInvalid(this, result.message);
                            this.setCustomValidity(result.message);
                        }
                    }
                }
            });
        });

        // Set initial custom validity for required fields to override browser defaults
        const allFields = form.querySelectorAll('input[name], textarea[name], select[name]');
        allFields.forEach(field => {
            const name = field.getAttribute('name');
            if (name && validators[name]) {
                const val = field.value;
                const result = validators[name](val);
                if (!result.valid) {
                    field.setCustomValidity(result.message);
                } else {
                    field.setCustomValidity('');
                }
            } else {
                field.setCustomValidity('');
            }
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
                const name = field.getAttribute('name');
                // For Message field, only remove dangerous chars, don't trim
                if (name === 'Message') {
                    field.value = sanitizeMessage(field.value);
                } else {
                    field.value = sanitize(field.value);
                }
                clearInvalid(field);
                field.setCustomValidity('');

                const val = field.value;
                
                if (validators[name]) {
                    const result = validators[name](val);
                    if (!result.valid) {
                        allValid = false;
                        setInvalid(field, result.message);
                    } else {
                        field.setCustomValidity('');
                    }
                } else {
                    field.setCustomValidity('');
                }
            });

            if (allValid) {
                form.submit();
            }
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            setTimeout(initializeFormValidation, 100);
        });
    } else {
        setTimeout(initializeFormValidation, 100);
    }

    // Also initialize after a short delay to ensure all scripts are loaded
    window.addEventListener('load', function() {
        setTimeout(initializeFormValidation, 200);
    });
})();

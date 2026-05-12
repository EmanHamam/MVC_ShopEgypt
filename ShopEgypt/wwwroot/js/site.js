// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    const menuBtn = document.getElementById("mobileMenuBtn");
    const mobileNav = document.getElementById("mobileNav");

    if (menuBtn && mobileNav) {
        menuBtn.addEventListener("click", function () {
            if (mobileNav.style.display === "block") {
                mobileNav.style.display = "none";
            } else {
                mobileNav.style.display = "block";
            }
        });
    }

    const autoSubmitForm = document.querySelector(".product-filter-form[data-auto-submit='true']");
    if (autoSubmitForm) {
        const formId = autoSubmitForm.id;
        let submitTimer = null;
        const scheduleSubmit = () => {
            if (submitTimer) {
                clearTimeout(submitTimer);
            }
            submitTimer = setTimeout(() => {
                autoSubmitForm.requestSubmit();
            }, 350);
        };

        const fields = Array.from(autoSubmitForm.querySelectorAll("input, select"));
        if (formId) {
            fields.push(...document.querySelectorAll(`[form='${formId}']`));
        }

        fields.forEach((field) => {
            const isPriceField = field.name === "minPrice" || field.name === "maxPrice";

            if (field.type === "text") {
                field.addEventListener("input", scheduleSubmit);
            } else if (field.type === "number" && isPriceField) {
                field.addEventListener("change", scheduleSubmit);
            } else {
                field.addEventListener("change", scheduleSubmit);
            }
        });
    }

    const heroImage = document.querySelector(".product-hero-image img");
    document.querySelectorAll(".product-thumb").forEach((thumb) => {
        thumb.addEventListener("click", () => {
            const imageUrl = thumb.getAttribute("data-image");
            if (heroImage && imageUrl) {
                heroImage.src = imageUrl;
            }
        });
    });

    const qtyControl = document.querySelector(".qty-control");
    const addToBagQty = document.querySelector(".add-to-bag input[name='qty']");
    if (qtyControl && addToBagQty) {
        const qtyInput = qtyControl.querySelector("input[name='qty']");
        const min = parseInt(qtyControl.getAttribute("data-min") || "1", 10);

        const syncQty = (value) => {
            const safeValue = Math.max(min, value || min);
            qtyInput.value = safeValue;
            addToBagQty.value = safeValue;
        };

        qtyControl.querySelectorAll(".qty-btn").forEach((btn) => {
            btn.addEventListener("click", () => {
                const step = parseInt(btn.getAttribute("data-step") || "0", 10);
                const current = parseInt(qtyInput.value || "1", 10);
                syncQty(current + step);
            });
        });

        qtyInput.addEventListener("change", () => {
            const current = parseInt(qtyInput.value || "1", 10);
            syncQty(current);
        });
    }
});
// Loads the free time slots for the chosen service and day into the booking form.
(function () {
  var form = document.getElementById('booking-form');
  if (!form) return;

  if (window.jQuery && jQuery.validator) {
    jQuery.extend(jQuery.validator.messages, {
      min: 'Kies een dag vanaf vandaag.',
      max: 'Zo ver vooruit kan je nog niet boeken.',
      dateISO: 'Kies een geldige datum.'
    });
  }

  var slots = document.getElementById('slots');
  var dateInput = form.querySelector('input[type=date]');
  var selected = slots.dataset.selected || '';
  var request = 0;

  function serviceId() {
    var checked = form.querySelector('#service-pick input:checked');
    return checked ? checked.value : null;
  }

  function hint(text) {
    slots.innerHTML = '';
    var p = document.createElement('p');
    p.className = 'hint';
    p.textContent = text;
    slots.appendChild(p);
  }

  function skeleton() {
    slots.innerHTML = '';
    slots.setAttribute('aria-busy', 'true');
    for (var i = 0; i < 6; i++) {
      var s = document.createElement('span');
      s.className = 'skeleton';
      s.setAttribute('aria-hidden', 'true');
      slots.appendChild(s);
    }
  }

  // Re-read validation rules so the new radio buttons are validated client-side too.
  function refreshValidation() {
    if (!window.jQuery || !jQuery.validator) return;
    var $form = jQuery(form);
    $form.removeData('validator').removeData('unobtrusiveValidation');
    jQuery.validator.unobtrusive.parse($form);
  }

  function render(times, message) {
    slots.removeAttribute('aria-busy');
    if (!times.length) {
      hint(message || 'Geen vrije momenten op deze dag.');
      refreshValidation();
      return;
    }
    slots.innerHTML = '';
    times.forEach(function (t, i) {
      var label = document.createElement('label');
      var input = document.createElement('input');
      input.type = 'radio';
      input.name = 'Booking.StartTime';
      input.value = t;
      input.id = 'slot-' + t.replace(':', '');
      input.checked = t === selected;
      if (i === 0) {
        input.setAttribute('data-val', 'true');
        input.setAttribute('data-val-required', 'Kies een tijdstip.');
      }
      input.addEventListener('change', function () { selected = t; });
      var span = document.createElement('span');
      span.textContent = t;
      label.appendChild(input);
      label.appendChild(span);
      slots.appendChild(label);
    });
    refreshValidation();
  }

  function load() {
    var id = serviceId();
    var date = dateInput.value;
    if (!id || !date) {
      hint('Kies eerst een beurt en een dag. Open op donderdag, vrijdag en zaterdagvoormiddag.');
      return;
    }
    var current = ++request;
    skeleton();
    fetch(slots.dataset.url + '?date=' + encodeURIComponent(date) + '&serviceId=' + encodeURIComponent(id), { headers: { 'Accept': 'application/json' } })
      .then(function (r) { if (!r.ok) throw new Error(r.status); return r.json(); })
      .then(function (data) { if (current === request) render(data.slots, data.message); })
      .catch(function () { if (current === request) { slots.removeAttribute('aria-busy'); hint('De vrije momenten konden niet geladen worden. Probeer opnieuw of bel ons.'); } });
  }

  form.querySelectorAll('#service-pick input').forEach(function (i) { i.addEventListener('change', load); });
  dateInput.addEventListener('change', load);
  load();

  // Map skeleton: hide once the map has loaded.
  var map = document.querySelector('#map iframe');
  if (map) map.addEventListener('load', function () { map.parentElement.classList.add('loaded'); });
})();

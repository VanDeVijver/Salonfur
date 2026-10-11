// Admin reservation calendar (FullCalendar), fed by /admin/agenda.
(function () {
  var el = document.getElementById('calendar');
  if (!el || !window.FullCalendar) return;

  var card = document.getElementById('calendar-card');
  var narrow = window.matchMedia('(max-width: 640px)').matches;
  var hiddenDays = (el.dataset.hiddenDays || '').split(',').filter(Boolean).map(Number);

  // Open on the next opening day, so on e.g. a Sunday the coming week is shown.
  var initialDate = new Date();
  for (var i = 0; i < 7 && hiddenDays.indexOf(initialDate.getDay()) !== -1; i++) {
    initialDate.setDate(initialDate.getDate() + 1);
  }

  var calendar = new FullCalendar.Calendar(el, {
    locale: 'nl',
    initialView: narrow ? 'listWeek' : 'timeGridWeek',
    initialDate: initialDate,
    headerToolbar: narrow
      ? { left: 'prev,next', center: 'title', right: 'today' }
      : { left: 'prev,next today', center: 'title', right: 'timeGridWeek,dayGridMonth,listWeek' },
    hiddenDays: hiddenDays,
    slotMinTime: el.dataset.slotMin,
    slotMaxTime: el.dataset.slotMax,
    slotDuration: '00:30:00',
    allDaySlot: true,
    allDayText: '',
    nowIndicator: true,
    height: 'auto',
    events: el.dataset.eventsUrl,
    eventTimeFormat: { hour: '2-digit', minute: '2-digit', hour12: false },
    noEventsContent: 'Geen afspraken in deze periode',
    loading: function (isLoading) {
      if (!isLoading) card.classList.add('ready');
    }
  });

  calendar.render();
})();

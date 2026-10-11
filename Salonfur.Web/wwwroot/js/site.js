// Ask for confirmation before destructive form posts: <form data-confirm="Zeker?">
document.addEventListener('submit', function (e) {
  var message = e.target.getAttribute && e.target.getAttribute('data-confirm');
  if (message && !window.confirm(message)) e.preventDefault();
});

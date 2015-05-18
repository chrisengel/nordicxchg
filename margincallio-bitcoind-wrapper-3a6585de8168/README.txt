1. Виртуальный хост можно не делать, общаться по ip
2. Скопировать config/main.php.sample в config/main.php
3. Скопировать config/bitcoin.conf.sample в config/bitcoin.conf
4. Настроить config/main.php
5. Настроить config/bitcoin.conf
6. Создать папки
	runtime, дать права на запись
	runtime/logs, дать права на запись
7. Конфигурация nginx для bitcoin сервера
server {
	listen 80 default_server;

	root /usr/share/nginx/core.bitcoin/www;
	index index.html index.htm;

	server_name localhost;

	location / {
		try_files $uri $uri/ =404;
	}

	location ~ \.php$ {
		fastcgi_split_path_info ^(.+\.php)(/.+)$;
		# NOTE: You should have "cgi.fix_pathinfo = 0;" in php.ini
		fastcgi_pass unix:/var/run/php5-fpm.sock;
		fastcgi_index index.php;
		include fastcgi_params;
	}
}
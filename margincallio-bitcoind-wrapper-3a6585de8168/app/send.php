<?php
/**
 * Файл вызывается по cron и разбирает список задач на отправку средств.
 * По результату вызывается ['notify']['sent']
 */

$config = require __DIR__.'/../config/main.php';
require_once __DIR__.'/lib/easybitcoin.php';
require_once __DIR__.'/lib/Service.php';
require_once __DIR__.'/lib/Redis.php';

Service::log('info', '[send.php] Send script running');

$bitcoin = new Bitcoin(
    $config['bitcoind']['login'],
    $config['bitcoind']['password'],
    '127.0.0.1',
    $config['bitcoind']['port']
);

$redis = new Redis(
    '127.0.0.1',
    $config['redis']['port']
);

// собираем запросы на отправку
$requestsRaw =
$requests =
$transactionOrders = [];
while ($request = $redis->rPop('queueSend')) {
    $requestsRaw[] = $request;
    $request = json_decode($request, true);
    $requests[] = $request;
    $transactionOrders[] = $request['transactionOrder'];
}

if (empty($requests)) {
    Service::log('info', '[send.php] Queue is empty');
    die("Queue is empty\n");
}

$sendRequest = [];
foreach ($requests as $r) {
    if (isset($sendRequest[$r['address']])) {
        $sendRequest[$r['address']] = bcadd($sendRequest[$r['address']], $r['amount'], 8);
    }
    else {
        $sendRequest[$r['address']] = $r['amount'];
    }
}

$txid = $bitcoin->sendmany($sendRequest);

Service::log('info', '[send.php] Sent txid', $txid);

// отправка не удалась
if (!$txid || $bitcoin->error) {
    // вернем запрос в очередь
    $redis->rPush('queueSend', $requestsRaw);
    Service::log('crit', '[send.php] Не удалась отправка средств по причине', $bitcoin->response);
    die("Error: $bitcoin->error\n");
}

$tx = $bitcoin->gettransaction($txid);

if (!$tx) {
    Service::log('crit', '[send.php] Не удалась отправка средств txid', $txid, $tx);
    die("Error: $bitcoin->error\n");
}

// уведомим ядро об отправке денег
$response = Service::sendRequest(
    $config['notify']['sent'],
    [
        'transactionOrders' => $transactionOrders,
        'commission' => bcmul($tx['fee'], '-1', 8),
        'txid' => $txid
    ],
    $config['requestSalt']
);
if (!$response || !$response['success']) {
    Service::log('crit', '[send.php] Не удалось уведомить ядро о переводе наружу', $response);
}
die("Ready\n");
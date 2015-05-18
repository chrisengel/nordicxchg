<?php
/**
 * Файл вызывается, когда приходит новый блок.
 */

$config = require __DIR__.'/../config/main.php';
require_once __DIR__.'/lib/easybitcoin.php';
require_once __DIR__.'/lib/Service.php';
require_once __DIR__.'/lib/Redis.php';

if (empty($argv[1])) {
    die;
}

Service::log('info', "BlockNotify $argv[1]");

// Смотрим на транзакции за 7 суток
$testTime = 3600*24*7;

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

$confirmedTransactions = [];

$limit = 500;
$offset = 0;
$transactions = [];
do {
    $needMore = true;
    $tmp = $bitcoin->listtransactions('', $limit, $offset);
    if (empty($tmp)) {
        break;
    }
    foreach ($tmp as $tx) {
        // это не получение средств
        if ($tx['category'] != 'receive') {
            continue;
        }
        $transactions[] = $tx;
        /*
        if ($tx['time'] < time() - $testTime) {
            $needMore = false;
            break;
        }
        */
    }
    $offset += $limit;
} while ($needMore);

usort($transactions, function($a, $b) {
    if ($a['time'] == $b['time']) {
        return 0;
    }
    return $a['time'] < $b['time'] ? 1 : -1;
});

foreach ($transactions as $tx) {
    // прошло больше 7 суток
    if ($tx['time'] < time() - $testTime) continue;
    // меньше 3 подтверждений
    if ($tx['confirmations'] < 3) continue;
    // уже подтверждено
    if ($redis->Exists("tx:sent:$tx[txid]")) {
        continue;
    }

    $request = [
        'address' => $tx['address'],
        'amount' => $tx['amount'],
        'txid' => $tx['txid'],
    ];

    $response = Service::sendRequest($config['notify']['received'], $request, $config['requestSalt']);

    // какая-то фигня на сервере
    if (!is_array($response) || !$response['success']) {
        Service::log('warn', 'Некорректный ответ ядра на транзакцию', $request, $response);
        continue;
    }

    // создание этого файла обозначает, что транзакция обработана и повторная обработка не требуется
    $redis->SetEx("tx:sent:$tx[txid]", $testTime, '1');

    $confirmedTransactions[] = "$tx[address] <- $tx[amount]";
}

if ($confirmedTransactions) {
    Service::log('info', 'Подтверждены транзакции', $confirmedTransactions);
}

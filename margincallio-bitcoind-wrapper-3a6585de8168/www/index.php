<?php

$config = require '../config/main.php';
require_once '../app/lib/easybitcoin.php';
require_once '../app/lib/Service.php';
require_once '../app/lib/Redis.php';

function json(array $data) {
    echo json_encode($data, JSON_UNESCAPED_UNICODE);
    die;
}

/*if (!Service::checkRequest($config['requestSalt'])) {
    json(['success' => false, 'message' => 'Wrong sign for message']);
}*/

$request = $_POST['request'];

if (empty($request['action'])) {
    die;
}

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
//13MQDipHsmyz5JhCudBduih7Tf1LFZ21u4
header('Content-type: application/json');

// выдать новый адрес для получения средств
if ($request['action'] == 'getNewAddress') {
    $count = !empty($request['count']) && is_numeric($request['count']) && $request['count'] > 1 ? $request['count'] : 1;
    $result = [];
    for ($i = 1; $i <= $count; $i++) {
        $address = $bitcoin->getnewaddress();
        if (!$address) {
            Service::log('warn', 'Не удалось получить новый адрес', $bitcoin->error);
            break;
        }
        $result[] = $address;
    }
    json(['success' => !!count($result), 'data' => $result]);
}
// валидировать адрес
elseif ($request['action'] == 'validateAddress') {
    $address = !empty($request['address']) ? $request['address'] : '';
    $valid = $bitcoin->validateaddress($address);
    json(['success' => true, 'isValid' => !!$valid['isvalid']]);
}
// запросить выплату на указанный набор кошельков
elseif ($request['action'] == 'requestSend') {
    // формат request={"transactionOrder":6823,"address":"1q8fdusS97fs","amount":"0.9627"}
    if (empty($request) || !is_array($request)) {
        json(['success' => false, 'message' => 'Wrong format']);
    }
    if (empty($request['address']) || empty($request['amount']) || empty($request['transactionOrder'])) {
        json(['success' => false, 'message' => 'Wrong format of record "'.json_encode($request).'"']);
    }
    $valid = $bitcoin->validateaddress($request['address']);
    if (!$valid['isvalid']) {
        json(['success' => false, 'message' => 'Wrong format of address "'.$request['address'].'"']);
    }
    if (!preg_match('~^\d+(\.\d{1,8})?$~', $request['amount']) || bccomp($request['amount'], '0.0001', 8) == -1) {
        json(['success' => false, 'message' => 'Wrong amount "'.$request['amount'].'"']);
    }
    $isPushed = $redis->lPush('queueSend', json_encode([
        'transactionOrder' => $request['transactionOrder'],
        'address' => $request['address'],
        'amount' => $request['amount']
    ]));
    json(['success' => !!$isPushed]);
}

json(['success' => false, 'message' => 'Unknown command']);

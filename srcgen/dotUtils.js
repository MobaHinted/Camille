﻿const enumTable = {
  'champion':   'Camille.Enums.Champion',
  'division':   'Camille.Enums.Division',
  'gameMode':   'Camille.Enums.GameMode',
  'gameType':   'Camille.Enums.GameType',
  'locale':     'Camille.Enums.Locale',
  'map':        'Camille.Enums.Map',
  'queue':      'Camille.Enums.Queue',
  'queueType':  'Camille.Enums.QueueType',
  'tier':       'Camille.Enums.Tier',

  'team':       'Camille.RiotGames.Enums.Team',
};

function capitalize(input) {
  return input[0].toUpperCase() + input.slice(1);
}

function decapitalize(input) {
  return input[0].toLowerCase() + input.slice(1);
}

function escapeKeyword(name) {
  switch (name.toUpperCase()) {
    case 'PUBLIC':
      return '@' + name;
  }
  return name;
}

function removePrefix(name, prefix) {
  return name.startsWith(prefix) ? name.slice(prefix.length) : name;
}

function normalizeEndpointName(name) {
  return name.split('-')
    //    .slice(0, -1)
    .map(capitalize)
    .join('');
}

function normalizeSchemaName(name) {
  return name.replace(/DTO/ig, '');
}

function normalizeArgName(name) {
  var tokens = name.split('_');
  var argName = decapitalize(tokens.map(capitalize).join(''));
  return 'base' === argName ? 'Base' : argName;
}

function normalizePropName(propName, schemaName, value) {
  if (/^\d/.test(propName)) // No leading digits.
    propName = 'x' + propName;
  const tokens = propName.split(/[_-]/g);
  let name = tokens.map(capitalize).join('');
  if (name === schemaName)
    name += '_';
  return name;
}

function stringifyType(prop, endpoint = null) {
  if (!prop)
    return 'UNDEFINED_TYPE';

  // Use first field of anyOf. HACK.
  if (prop.anyOf)
    prop = prop.anyOf[0];

  let refType = prop['$ref'];
  if (refType) {
    refType = refType.split('/').pop();
    refType = refType.split('.').pop();
    return (endpoint ? endpoint + '.' : '') + normalizeSchemaName(refType);
  }
  const enumName = enumTable[prop['x-enum']];
  if (enumName !== undefined) {
    return enumName;
  }

  switch (prop.type) {
    case 'boolean': return 'bool';
    case 'integer': return ((!prop.format || 'int32' === prop.format) ? 'int' : 'long');
    case 'number': return prop.format || 'double';
    case 'array': return stringifyType(prop.items, endpoint) + '[]';
    case 'object': {
      if (1 === Object.keys(prop).length) { // Only `{ "type": "object" }`.
        return 'System.Text.Json.Nodes.JsonObject';
      }
      const keyType = prop['x-key'] ? stringifyType(prop['x-key'], endpoint) : 'string';
      return `IDictionary<${keyType}, ${stringifyType(prop.additionalProperties, endpoint)}>`;
    }
    default: return prop.type || 'Dictionary<string, object>';
  }
}

function stringifyParam(param, endpoint = null) {
  if (param.schema)
    return stringifyType(param.schema, endpoint);

  const schema = { ...param }; // Clone.
  if (param.enum)
    schema['x-enum'] = param.name; // HACKY.
  return stringifyType(schema);
}

function replaceEnumCasts(input) {
  return input.replace("{championId}", "{(int)championId}");
}

function formatJsonProperty(name) {
  return `[JsonPropertyName("${name}")]`;
}

function formatQueryParamStringify(expr, prop) {
  if (prop['x-enum']) {
    switch (prop['x-type']) {
        case 'int': return `((int) ${expr}).ToString()`;
        case 'String': return `${expr}.ToString()`;
        default: throw Error(`Unexpected x-enum x-type: ${prop['x-type']}\n${JSON.stringify(prop)}`);
    }
  }
  if (prop['$ref']) {
    const type = prop['$ref'].split('/').pop();
    console.error("Cannot put unknown type in query param: " + type);
    return 'BAD_QP_TYPE';
  }
  switch (prop.type) {
    case undefined: throw new Error('Undefined .type: ' + JSON.stringify(prop));
    case 'boolean': return expr + '.ToString().ToLowerInvariant()';
    case 'string': return expr;
    default:
      return expr + '.ToString()';
  }
}

function formatAddQueryParam(param) {
  let prop = param.schema;
  if (!prop) {
    // HACK. Some LCU endpoints don't obey the openapi spec and have
    // ".schema" values directly in param.
    return formatAddQueryParam({ ...param, schema: param })
  }

  let k = `nameof(${param.name})`;
  let nc = param.required ? '' : `if (null != ${param.name}) `;

  switch (prop.type) {
    case 'array': return `${nc}Array.ForEach(${param.name}, w => queryParams.Add(${k}, ${formatQueryParamStringify('w', prop.items)}))`;
    case 'object': throw new Error('Object unsupported for query param.');
    default: {
      let expr = param.name + ((param.required || 'string' === prop.type) ? '' : '.Value');
      return `${nc}queryParams.Add(${k}, ${formatQueryParamStringify(expr, prop)})`;
    }
  }
}

function formatComment(comment, indent) {
  return comment.split('\n').map(x => x.trim()).join(`<para />\r\n${' '.repeat(indent)}/// `)
}

module.exports = {
  capitalize,
  decapitalize,
  escapeKeyword,
  removePrefix,
  normalizeEndpointName,
  normalizeSchemaName,
  normalizeArgName,
  normalizePropName,
  stringifyType,
  stringifyParam,
  replaceEnumCasts,
  formatJsonProperty,
  formatAddQueryParam,
  formatComment
};